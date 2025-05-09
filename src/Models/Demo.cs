namespace MyApp.Models;

public class DemoItem {
  public int Id { get; set; }
  public string Title { get; set; } = "";
  public bool IsCompleted { get; set; }
}

public record DemoDto(string Title, bool IsCompleted);
