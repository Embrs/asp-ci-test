using MyApp.Dtos;
using MyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Endpoints;

public static class AuthEndpoints {
	public static void MapAuthEndpoints(this WebApplication app) {
		app.MapPost("/auth/register", async (
			AppDbContext db,
			RegisterDto dto
		) => {
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
	}
}
