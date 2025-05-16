namespace MyApp.Apis;

using MyApp.ApiParams;
using MyApp.Models;
using MyApp.Db;
using Microsoft.EntityFrameworkCore;
using MyApp.Services;
using MyApp.Filters;
using Microsoft.AspNetCore.Mvc;

public static class AuthApis {
	public static WebApplication InitBaseApis(this WebApplication app) {
		var baseGroup = app.MapGroup("/api/base");
		baseGroup.MapPost("/sign-up", ApiSignUp);
		baseGroup.MapPost("/sign-in", ApiSignIn);
		baseGroup.MapPost("/logout", ApiLogout);
		baseGroup.MapGet("/self/info", ApiGetSelfInfo).AddEndpointFilter<RequireUserFilter>();
		return app;
	}

	/** 註冊 */
	private static async Task<IResult> ApiSignUp(
		[FromServices] AppDbContext db,
		[FromBody] SignUpParams apiParams
	) {
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
	private static async Task<IResult> ApiSignIn(
		HttpContext context,
		[FromServices] AppDbContext db,
		[FromServices] RedisService redisService,
		[FromBody] SignInParams apiParams
	) {
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
		var token = await redisService.CreateToken(userInfo, context);
		return Results.Ok(new {
			token,
			userInfo.PublicId,
			userInfo.DisplayName,
		});
	}

  /** 登出 */
  private static async Task<IResult> ApiLogout(
    HttpContext context,
		[FromServices] RedisService redisService
  )
  {
    await redisService.DeleteToken(context);
    return Results.Ok();
  }

  /** 取得自己的資料 */
  private static IResult ApiGetSelfInfo(
		HttpContext context, 
		[FromServices] AppDbContext db
	) {
		var tokenUser = context.Items["tokenUser"] as User;
    if (tokenUser is null) return Results.Unauthorized();
		return Results.Ok(new {
			tokenUser.DisplayName,
			tokenUser.PublicId,
		});
	}
}