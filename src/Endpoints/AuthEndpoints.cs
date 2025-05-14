using MyApp.Dtos;
using MyApp.Models;
using MyApp.Db;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Endpoints;

public static class AuthEndpoints {
	public static void MapAuthEndpoints(this WebApplication app) {

		// Register
		app.MapPost("/auth/register", async (AppDbContext db,RegisterDto dto) => {
			if (await db.UserCredentials.AnyAsync(c => c.Provider == "local" && c.Identifier == dto.Email)) {
				return Results.Conflict("Email already registered.");
			}

			var user = new User {
				DisplayName = dto.DisplayName,
				Credentials = [
					new UserCredential {
						Provider = "local",
						Identifier = dto.Email,
						PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
					}
				]
			};

			db.Users.Add(user);
			await db.SaveChangesAsync();

			return Results.Created($"/users/{user.PublicId}", new {
				user.PublicId,
				user.DisplayName,
			});
		});
	
		// Login
		app.MapPost("/auth/login", async (AppDbContext db, LoginDto dto) => {
			var credential = await db.UserCredentials
				.Include(uc => uc.User)
				.FirstOrDefaultAsync(uc =>
					uc.Provider == "local" &&
					uc.Identifier == dto.Email);

			if (credential == null || !BCrypt.Net.BCrypt.Verify(dto.Password, credential.PasswordHash)) {
				return Results.Unauthorized();
			}

			return Results.Ok(
				new {
					credential.User.PublicId,
					credential.User.DisplayName
				}
			);
		});
	
	}
}
