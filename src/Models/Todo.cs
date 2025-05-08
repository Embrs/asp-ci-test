namespace MyApp.Models;

public class Todo {
  public int Id { get; set; }
  public string Title { get; set; } = "";
  public bool IsDone { get; set; }
}

public record TodoDto(string Title, bool IsDone);
