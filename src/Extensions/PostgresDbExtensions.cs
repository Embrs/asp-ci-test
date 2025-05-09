namespace MyApp.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Data;
public static class DbExtensions {  
  // 註冊 DbContext
  public static IServiceCollection AddPostgresDb (this IServiceCollection services, IConfiguration configur) {
    try {
      services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configur.GetConnectionString("DefaultConnection")));
    } catch (Exception) {}
    return services;
  }

  // 建立資料庫
  public static void UsePostgresDb (this IHost host) {
    try {
      using var scope = host.Services.CreateScope();
      var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      db.Database.Migrate();
    } catch (Exception) { }
  }
}