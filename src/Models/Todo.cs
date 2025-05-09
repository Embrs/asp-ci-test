namespace MyApp.Models;
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public bool IsCompleted { get; set; }
}

public record TodoDto(string Title, bool IsCompleted);