namespace MyApp.Models;

public class User {
  public int Id { get; set; }

  public Guid PublicId { get; set; } = Guid.NewGuid();

  public required string DisplayName { get; set; }


  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public ICollection<UserCredential> Credentials { get; set; } = [];
}
