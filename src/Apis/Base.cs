namespace MyApp.Apis;

using MyApp.ApiParams;
using MyApp.Models;
using MyApp.Db;
using Microsoft.EntityFrameworkCore;
using MyApp.Services;
using System.Security.Claims;


public static class AuthApis {
	public static WebApplication InitBaseApis(this WebApplication app) {
		var baseGroup = app.MapGroup("/api/base/");
		baseGroup.MapPost("sign-up", ApiSignUp);
		baseGroup.MapPost("sign-in", ApiSignIn);
		baseGroup.MapGet("self/info", ApiGetSelfInfo).RequireAuthorization();;
		return app;
	}

	private static async Task<IResult> ApiSignUp(AppDbContext db, RegisterParams apiParams) {
		if (await db.UserCredentials.AnyAsync(c => c.Provider == "local" && c.Identifier == apiParams.Email)) {
			return Results.Conflict("Email already registered.");
		}

		var user = new User {
			DisplayName = apiParams.DisplayName,
			Credentials = [
				new UserCredential {
					Provider = "local",
					Identifier = apiParams.Email,
					PasswordHash = BCrypt.Net.BCrypt.HashPassword(apiParams.Password),
				}
			]
		};

		db.Users.Add(user);
		await db.SaveChangesAsync();

		return Results.Created($"/users/{user.PublicId}", new {
			user.PublicId,
			user.DisplayName,
		});
	}

	private static async Task<IResult> ApiSignIn(AppDbContext db, LoginParams apiParams, JwtService jwtService) {
		var credential = await db.UserCredentials
			.Include(uc => uc.User)
			.FirstOrDefaultAsync(uc =>
				uc.Provider == "local" &&
				uc.Identifier == apiParams.Email);

		if (credential == null || !BCrypt.Net.BCrypt.Verify(apiParams.Password, credential.PasswordHash)) {
			return Results.Unauthorized();
		}

		var user = credential.User;
		var token = jwtService.GenerateToken(user);

		return Results.Ok(new {
			token,
			user.PublicId,
			user.DisplayName,
		});
	}

	private static async Task<IResult> ApiGetSelfInfo(HttpContext context, AppDbContext db) {

		var authHeader = context.Request.Headers.Authorization.ToString();

		if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) {
			return Results.Unauthorized();
		}

		var token = authHeader.Substring("Bearer ".Length);

		var user = context.User;

		var sub =user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;


		if (sub == null || !Guid.TryParse(sub, out var publicId)) {
			return Results.Unauthorized();
		}

		var u = await db.Users.FirstOrDefaultAsync(u => u.PublicId == publicId);
		if (u == null) return Results.NotFound();

		return Results.Ok(new {
			u.DisplayName,
			u.PublicId,
		});
	}
}