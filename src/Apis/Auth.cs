namespace MyApp.Apis;

using MyApp.ApiParams;
using MyApp.Models;
using MyApp.Db;
using Microsoft.EntityFrameworkCore;
using MyApp.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class AuthApis {
	public static void InitAuthApis(this WebApplication app) {
		var group = app.MapGroup("/auth");

		group.MapPost("/register", RegisterAsync);
		group.MapPost("/login", LoginAsync);
		group.MapGet("/me", GetMeAsync);
	}

	private static async Task<IResult> RegisterAsync(AppDbContext db, RegisterParams apiParams) {
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

	private static async Task<IResult> LoginAsync(AppDbContext db, LoginParams apiParams, JwtService jwtService) {
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

	private static async Task<IResult> GetMeAsync(ClaimsPrincipal user, AppDbContext db) {
		var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
		Console.WriteLine($"[InitJwt sub] {sub}");

		if (sub == null || !Guid.TryParse(sub, out var publicId)) {
			Console.WriteLine($"[InitJwt null] ");
			return Results.Unauthorized();
		}

		var u = await db.Users.FirstOrDefaultAsync(u => u.PublicId == publicId);
		Console.WriteLine($"[InitJwt user] {u}");
		if (u == null) {
			Console.WriteLine($"[InitJwt user null]");
			return Results.NotFound();
		}

		return Results.Ok(new {
			u.DisplayName,
			u.PublicId,
		});
	}
}