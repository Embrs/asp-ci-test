namespace MyApp.Models;
public class TodoItem {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty; // ✅ 預設為空字串
    public bool IsCompleted { get; set; }
}

public record TodoDto(string Title, bool IsCompleted);