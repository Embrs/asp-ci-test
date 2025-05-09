namespace MyApp.Extensions;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApp.Repositories;
using MyApp.Endpoints;
using MyApp.Models;

public static class ServiceCollectionExtensions {

  /** 註冊 CORS 服務 */
  public static IServiceCollection AddCorsServices(this IServiceCollection services) {
    services.AddCors(options => {
      options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()  // 允許來自任何來源的請求
              .AllowAnyHeader()  // 允許任何標頭
              .AllowAnyMethod(); // 允許任何方法（GET、POST 等）
      });
    });
    return services;
  }

  /** 註冊 JWT 服務 */
  public static IServiceCollection AddJwtServices(this IServiceCollection services, ConfigurationManager configur) {
    services.Configure<JwtSettings>(configur.GetSection("Jwt"));
    var jwtSettings = configur.GetSection("Jwt").Get<JwtSettings>();
    if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
      throw new InvalidOperationException("JWT settings are not configured properly.");
        
    var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

    // 🔹 加入 JWT 認證
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
      options.TokenValidationParameters = new() {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
      };
    });

    services.AddAuthorization();

    // 🔹 加入 Swagger 並設定 JWT Bearer 權限描述
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
    return services;
  }

  /** 註冊應用程式服務 */
  public static IServiceCollection AddApiServices(this IServiceCollection services) {
    services.AddSingleton<DemoRepository>();
    services.AddSingleton<UserRepository>();
    // TODO add New service
    return services;
  }


  /** Swagger中間件 */
  public static WebApplication UseSwaggerMiddlewares(this WebApplication app) {
    app.UseSwagger();
    app.UseSwaggerUI(options => {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    });
    return app;
  }

  /** CORS 中間件 */
  public static WebApplication UseCorsMiddlewares(this WebApplication app) {
    app.UseCors(policy =>
      policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    );
    return app;
  }
 
  /** JWT 中間件 */
  public static WebApplication UseJwtMiddlewares(this WebApplication app) {
    app.UseAuthentication();
    app.UseAuthorization();
    return app;
  }

  /** 應用程式中間件 */
  public static WebApplication UseApiMiddlewares(this WebApplication app) {
    app.MapDemoEndpoints();
    app.MapAuthEndpoints();
    app.MapUserEndpoints();
    // TODO add New api
    return app;
  }
}
