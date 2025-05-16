namespace MyApp.Apis;

using MyApp.ApiParams;
using MyApp.Models;
using MyApp.Db;
using Microsoft.EntityFrameworkCore;
using MyApp.Services;


public static class AuthApis {
	public static WebApplication InitBaseApis(this WebApplication app) {
		var baseGroup = app.MapGroup("/api/base");
		baseGroup.MapPost("/sign-up", ApiSignUp);
		baseGroup.MapPost("/sign-in", ApiSignIn);
		baseGroup.MapPost("/logout", ApiLogout);
		baseGroup.MapGet("/self/info", ApiGetSelfInfo);
		return app;
	}

	/** 註冊 */
	private static async Task<IResult> ApiSignUp(AppDbContext db, RegisterParams apiParams) {
		if (await db.UserCredentials.AnyAsync(c => c.Provider == "local" && c.Identifier == apiParams.Email)) {
			return Results.Conflict("Email already registered.");
		}
		// 1. 建立使用者
		var userInfo = new User {
			DisplayName = apiParams.DisplayName,
			Credentials = [
				new UserCredential {
					Provider = "local",
					Identifier = apiParams.Email,
					PasswordHash = BCrypt.Net.BCrypt.HashPassword(apiParams.Password),
				}
			]
		};
		// 2. 加入 
		db.Users.Add(userInfo);
		await db.SaveChangesAsync();

		return Results.Created($"/users/{userInfo.PublicId}", new {
			userInfo.PublicId,
			userInfo.DisplayName,
		});
	}

	/** 登入 */
	private static async Task<IResult> ApiSignIn(AppDbContext db, LoginParams apiParams, JwtService jwtService) {
		// 1. 尋找用戶
		var credential = await db.UserCredentials
			.Include(uc => uc.User)
			.FirstOrDefaultAsync(uc =>
				uc.Provider == "local" &&
				uc.Identifier == apiParams.Email);

		// 2. 檢查密碼
		if (
			credential == null || 
			!BCrypt.Net.BCrypt.Verify(apiParams.Password, credential.PasswordHash)
		) return Results.Unauthorized();

		// 3. 取得使用者
		var userInfo = credential.User;

		// 4. 產生 token
		var token = jwtService.GenerateToken(userInfo);

		return Results.Ok(new {
			token,
			userInfo.PublicId,
			userInfo.DisplayName,
		});
	}

	/** 登出 */ // TODO
	private static IResult ApiLogout() => Results.Ok();

	/** 取得自己的資料 */
	private static async Task<IResult> ApiGetSelfInfo(HttpContext context, AppDbContext db, JwtService jwtService) {
		// 1. 取得 JWT token 中的 user id
		var tokenUserId = jwtService.CheckToken(context);
		if (tokenUserId == null) return Results.Unauthorized();

		// 2. 根據 user id 取得使用者資料
		var userInfo = await db.Users.FirstOrDefaultAsync(u => u.PublicId == tokenUserId);
		if (userInfo == null) return Results.NotFound();

		return Results.Ok(new {
			userInfo.DisplayName,
			userInfo.PublicId,
		});
	}
}