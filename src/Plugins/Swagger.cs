namespace MyApp.Plugins;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

public static class SwaggerPlugins {  
  // Swegger
  public static IServiceCollection SettingSwagger (this IServiceCollection services, IConfiguration configur) {
    try {
      services.AddEndpointsApiExplorer();
      
      services.AddSwaggerGen(options => {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });

        var jwtScheme = new OpenApiSecurityScheme {
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.Http,
          Scheme = "bearer",
          BearerFormat = "JWT",
          Description = "輸入以 `Bearer <token>` 的格式",
          Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme,
          },
        };

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement {{ jwtScheme, Array.Empty<string>() }});
      });
    }
    catch (Exception) {
      Console.WriteLine($"[SettingSwagger ERROR]"); 
    }
    Console.WriteLine($"[SettingSwagger OK]"); 
    return services;
  }

  public static WebApplication InitSwagger(this WebApplication app) {
    try {
      app.UseSwagger();
      app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
      });
    }
    catch (Exception) {
      Console.WriteLine($"[InitSwagger ERROR]"); 
    }
    Console.WriteLine($"[InitSwagger OK]"); 
    return app;
  }
}