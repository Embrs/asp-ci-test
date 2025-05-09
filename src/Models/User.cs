namespace MyApp.Models;

public class UserItem {
  public int Id { get; set; }
  public string Username { get; set; } = "";
  public string Email { get; set; } = "";
}

public record UserDto(string Username, string Email);
