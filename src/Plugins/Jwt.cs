
namespace MyApp.Plugins;

using System.IdentityModel.Tokens.Jwt;
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
      
      var secretBytes = Convert.FromBase64String(jwtConfig.SecretKey);
      // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // 🧨 禁用預設 Claim type 映射

      services.AddAuthentication(options =>  {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
      })
      .AddJwtBearer("Bearer", options => {
        options.RequireHttpsMetadata = false; // ← 本地開發建議關閉
        options.TokenValidationParameters = new TokenValidationParameters  {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtConfig.Issuer,
          ValidAudience = jwtConfig.Audience,
          IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
          ClockSkew = TimeSpan.Zero, // 👈 建議明確設定
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
      app.UseRouting(); 
      app.UseAuthentication();
      app.UseAuthorization();   
      // app.Use(async (ctx, next) => {
      //   Console.WriteLine($"[Middleware] Authenticated: {ctx.User.Identity?.IsAuthenticated}");
      //   Console.WriteLine($"[Middleware] Name: {ctx.User.Identity?.Name}");
      //   Console.WriteLine($"[Middleware] Claims: {string.Join(", ", ctx.User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
      //   Console.WriteLine($"[Middleware] sub: {ctx.User.FindFirst("sub")?.Value}");

      //   await next();
      // });
    } catch (Exception ex) {
      var env = app.Environment;
      Console.WriteLine($"[InitJwt ERROR] {env.EnvironmentName}: {ex.Message}");  
    }
    Console.WriteLine($"[InitJwt OK]");
    return app;
  }
}