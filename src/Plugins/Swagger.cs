namespace MyApp.Plugins;

using Microsoft.OpenApi.Models;

public static class SwaggerPlugins {
  public static IServiceCollection AddSwagger(this IServiceCollection services) {
    try {
      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen(options => {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });

        var tokenScheme = new OpenApiSecurityScheme {
          Name = "Authorization",
          Type = SecuritySchemeType.Http, // ✅ 使用 Http 類型
          Scheme = "bearer",              // ✅ 小寫 "bearer"
          // BearerFormat = "JWT",           // 可選，加強說明用途
          In = ParameterLocation.Header,
          Description = "請輸入 token，例如：Bearer <value>",
          Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer",
          },
        };

        options.AddSecurityDefinition("Bearer", tokenScheme);

        options.AddSecurityRequirement(new OpenApiSecurityRequirement {
          { tokenScheme, Array.Empty<string>() }
        });
      });
    }
    catch (Exception) {
      Console.WriteLine($"[AddSwagger ERROR]");
    }
    Console.WriteLine($"[AddSwagger OK]");
    return services;
  }

  public static WebApplication InitSwagger(this WebApplication app) {
    try {
      app.UseSwagger();
      app.UseSwaggerUI(options =>
      {
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
