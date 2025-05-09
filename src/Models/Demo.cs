namespace MyApp.Models;

public class DemoItem {
  public int Id { get; set; }
  public string Title { get; set; } = "";
  public bool IsComplete { get; set; }
}

public record DemoDto(string Title, bool IsComplete);
