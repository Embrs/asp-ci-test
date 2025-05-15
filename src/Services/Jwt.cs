namespace MyApp.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MyApp.Models;
using Microsoft.IdentityModel.Tokens;
using MyApp.Config;
using MyApp.Db;
using Microsoft.EntityFrameworkCore;

public class JwtService {
  private readonly JwtConfig _config;

  public JwtService(JwtConfig config) {
    _config = config;
  }


  public string GenerateToken(User user) {
    var claims = new[] {
      new Claim(JwtRegisteredClaimNames.Sub, user.PublicId.ToString()),
      new Claim("name", user.DisplayName),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
    var secretBytes = Convert.FromBase64String(_config.SecretKey);
    var key = new SymmetricSecurityKey(secretBytes);
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _config.Issuer,
      audience: _config.Audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(_config.ExpireMinutes),
      signingCredentials: creds
    );
    var sToken = new JwtSecurityTokenHandler().WriteToken(token);
    return sToken;
  }

  public Guid? CheckToken(HttpContext context) {
    // var authHeader = context.Request.Headers.Authorization.ToString();
    // var token = authHeader.Substring("Bearer ".Length);
    // var principal = tokenValidator.ValidateToken(token);

		var cUser = context.User;
    var sub = cUser.FindFirst("sub")?.Value ?? cUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (sub == null || !Guid.TryParse(sub, out var userId)) return null;
    return userId;
  }

}
