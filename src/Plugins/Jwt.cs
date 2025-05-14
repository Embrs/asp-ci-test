
namespace MyApp.Plugins;

using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyApp.Config;
using MyApp.Services;

public static class JwtPlugins {  
  // JWT 設定
  public static void SettingJwt (this IServiceCollection services, IConfiguration configur) {
    try {
      var jwtSection = configur.GetSection("Jwt");
      var jwtConfig = jwtSection.Get<JwtConfig>()!;
      services.AddSingleton(jwtConfig);
      services.AddSingleton<JwtService>();
      services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtConfig.Issuer,
          ValidAudience = jwtConfig.Audience,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
        };
      });
    } catch (Exception) {}
  } 

  // 建立資料庫
  public static void InitJwt (this WebApplication app) {
    try {
      app.UseAuthentication();
      app.UseAuthorization();
    } catch (Exception) {}
  }
}