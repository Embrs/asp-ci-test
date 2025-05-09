using MyApp.Models;

namespace MyApp.Repositories;

public class DemoRepository {
  private readonly List<DemoItem> _demoList = [];
  private int _nextId = 1;

  public List<DemoItem> GetAll() => _demoList;

  public DemoItem? GetById(int id) => _demoList.FirstOrDefault(t => t.Id == id);

  public DemoItem Create(string title) {
    var demoItem = new DemoItem { Id = _nextId++, Title = title, IsComplete = false };
    _demoList.Add(demoItem);
    return demoItem;
  }

  public bool Update(int id, string title, bool IsComplete) {
    var demoItem = GetById(id);
    if (demoItem == null) return false;

    demoItem.Title = title;
    demoItem.IsComplete = IsComplete;
    return true;
  }

  public bool Delete(int id) {
    var demoItem = GetById(id);
    return demoItem != null && _demoList.Remove(demoItem);
  }
}
