namespace MyApp.Endpoints;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyApp.Models;

public static class AuthEndpoints {
	public static void MapAuthEndpoints(this IEndpointRouteBuilder app) {
		app.MapPost("/api/auth/login", Login);
	}

	static IResult Login(UserLoginRequest request, IOptions<JwtSettings> jwtOptions) {
		// 🚧 模擬帳號密碼驗證（實際應查資料庫）
		if (request.Username != "admin" || request.Password != "1234") {
				return Results.Unauthorized();
		}

		var settings = jwtOptions.Value;
		Console.WriteLine($"JWT SecretKey = '{settings.SecretKey}'");
		if (string.IsNullOrWhiteSpace(settings.SecretKey)) {
				return Results.Problem("JWT SecretKey is not configured.");
		}

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new[] {
				new Claim(JwtRegisteredClaimNames.Sub, request.Username),
				new Claim("role", "admin"),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(settings.ExpireMinutes),
				signingCredentials: creds
		);

		var jwt = new JwtSecurityTokenHandler().WriteToken(token);
		return Results.Ok(new { token = jwt });
	}
}
