
namespace MyApp.Plugins;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApp.Config;
using MyApp.Services;

public static class JwtPlugins {  
  // JWT 設定
  public static IServiceCollection SettingJwt (this IServiceCollection services, IConfiguration configur) {
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

        // 加入以下錯誤日誌設定
        options.Events = new JwtBearerEvents {
          OnAuthenticationFailed = context => {
            Console.WriteLine($"[JWT ERROR] {context.Exception.Message}");
            return Task.CompletedTask;
          },
          OnChallenge = context => {
            Console.WriteLine($"[JWT CHALLENGE] {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
          }
        };

      });
      services.AddAuthorization();
    } catch (Exception) {}
    return services;
  } 

  // 建立資料庫
  public static WebApplication InitJwt (this WebApplication app) {
    try {
      app.UseAuthentication();
      app.UseAuthorization();
    } catch (Exception) {}
    return app;
  }
}