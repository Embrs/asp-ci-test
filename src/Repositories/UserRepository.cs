using MyApp.Models;

namespace MyApp.Repositories;

public class UserRepository {
  private readonly List<UserItem> _userList = [];
  
  private int _nextId = 1;

  public List<UserItem> GetAll() => _userList;

  public UserItem? GetById(int id) => _userList.FirstOrDefault(u => u.Id == id);

  public UserItem Create(string username, string email) {
    var userItem = new UserItem { Id = _nextId++, Username = username, Email = email };
    _userList.Add(userItem);
    return userItem;
  }

  public bool Update(int id, string username, string email) {
    var userItem = GetById(id);
    if (userItem == null) return false;

    userItem.Username = username;
    userItem.Email = email;
    return true;
  }

  public bool Delete(int id) {
    var userItem = GetById(id);
    return userItem != null && _userList.Remove(userItem);
  }
}
