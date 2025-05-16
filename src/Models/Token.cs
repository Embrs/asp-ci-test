namespace MyApp.Models;

public class TokenInfo
{
  public Guid userId { get; set; }
  public string ip { get; set; } =  string.Empty;
  public DateTime expiresAt { get; set; }
}