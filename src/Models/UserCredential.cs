namespace MyApp.Models;
public class UserCredential {
  public int Id { get; set; }

  public Guid PublicId { get; set; } = Guid.NewGuid();

  public required string Provider { get; set; }

  public required string Identifier { get; set; }

  public string? PasswordHash { get; set; }

  public int UserId { get; set; }

  public User User { get; set; } = null!;
}


