namespace MyApp.Plugins;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

public static class SwaggerPlugins {  
  // Swegger
  public static void SettingSwagger (this IServiceCollection services, IConfiguration configur) {
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(options => {
      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "JWT 授權 (輸入格式: Bearer {token})",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
      });

      options.AddSecurityRequirement(new OpenApiSecurityRequirement {{
        new OpenApiSecurityScheme {
          Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          }
        },
        Array.Empty<string>()
      }});
  });
  }

  public static void InitSwagger(this WebApplication app) {
    app.UseSwagger();
    app.UseSwaggerUI(options => {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    });
  }
}