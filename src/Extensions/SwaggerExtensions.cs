using Microsoft.OpenApi.Models;

public static class SwaggerExtensions {  
  // Swegger
  public static void SettingSwagger (this IServiceCollection services, IConfiguration configur) {
    services.AddSwaggerGen(options => {
      options.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });
    });
  }

  public static void InitSwagger(this WebApplication app) {
    app.UseSwagger();
    app.UseSwaggerUI(options => {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    });
  }
}