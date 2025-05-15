
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

      services.AddAuthentication(options =>  {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
      })
      .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters  {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtConfig.Issuer,
          ValidAudience = jwtConfig.Audience,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
        };

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
      // ✅ 這行是解決錯誤的關鍵
      services.AddAuthorization(); 
      // ✅ 加入這行以註冊 JwtService
      services.AddScoped<JwtService>();
    }
    catch (Exception) {
      Console.WriteLine($"[SettingJwt ERROR]");  
    }
    Console.WriteLine($"[SettingJwt OK]");
    return services;
  } 

  // 建立資料庫
  public static WebApplication InitJwt (this WebApplication app) {
    try {
      app.UseAuthentication();
      app.UseAuthorization();
    } catch (Exception ex) {
      var env = app.Environment;
      Console.WriteLine($"[InitJwt ERROR] {env.EnvironmentName}: {ex.Message}");  
    }
    Console.WriteLine($"[InitJwt OK]");
    return app;
  }
}