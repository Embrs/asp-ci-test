namespace MyApp.Plugins;

using Microsoft.EntityFrameworkCore;
using MyApp.Db;
using MyApp.Models;

public static class PostgresDbPlugins {
  
  // Postgres Db
  public static IServiceCollection SettingPostgresDb (this IServiceCollection services, IConfiguration configur) {
    try {
      services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configur.GetConnectionString("DefaultConnection")));
    } catch (Exception) {
      Console.WriteLine($"[SettingPostgresDb ERROR]");  
    }
    Console.WriteLine($"[SettingPostgresDb OK]");
    return services;
  }

  // 建立資料庫
  public static WebApplication InitPostgresDb (this WebApplication app) {
    try {
      using var scope = app.Services.CreateScope();
      var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

      db.Database.Migrate();

      if (!db.Users.Any()) {
        var user = new User {
          DisplayName = "Admin",
          Credentials = new List<UserCredential> {
            new UserCredential {
              Provider = "local",
              Identifier = "admin@example.com",
              PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            }
          }
        };

        db.Users.Add(user);
        db.SaveChanges();
      }
    } catch (Exception) {
      Console.WriteLine($"[InitPostgresDb ERROR]"); 
    }
    Console.WriteLine($"[InitPostgresDb OK]");
    return app;
  }
}