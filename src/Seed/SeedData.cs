using MyApp.Models;

namespace MyApp;

public static class SeedData {
  public static void Seed(this WebApplication app) {
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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
  }
}
