using MyApp.Models;

namespace MyApp.Repositories;

public class TodoRepository {
  private readonly List<Todo> _todos = [];
  private int _nextId = 1;

  public IEnumerable<Todo> GetAll() => _todos;

  public Todo? GetById(int id) => _todos.FirstOrDefault(t => t.Id == id);

  public Todo Create(string title) {
    var todo = new Todo { Id = _nextId++, Title = title, IsDone = false };
    _todos.Add(todo);
    return todo;
  }

  public bool Update(int id, string title, bool isDone) {
    var todo = GetById(id);
    if (todo == null) return false;

    todo.Title = title;
    todo.IsDone = isDone;
    return true;
  }

  public bool Delete(int id) {
    var todo = GetById(id);
    return todo != null && _todos.Remove(todo);
  }
}
