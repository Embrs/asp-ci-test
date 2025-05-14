using Microsoft.EntityFrameworkCore;
using MyApp.Db;
using MyApp.Models;

namespace MyApp.Extensions;

public static class PostgresDbExtensions {  
  // Postgres Db
  public static void SettingPostgresDb (this IServiceCollection services, IConfiguration configur) {
    try {
      services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configur.GetConnectionString("DefaultConnection")));
    } catch (Exception) {}
  }

  // 建立資料庫
  public static void InitPostgresDb (this WebApplication app) {
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
    } catch (Exception) { }
  }
}