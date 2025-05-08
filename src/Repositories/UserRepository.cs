using MyApp.Models;

namespace MyApp.Repositories;

public class UserRepository {
  private readonly List<User> _users = [];
  private int _nextId = 1;

  public IEnumerable<User> GetAll() => _users;

  public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

  public User Create(string username, string email) {
    var user = new User { Id = _nextId++, Username = username, Email = email };
    _users.Add(user);
    return user;
  }

  public bool Update(int id, string username, string email) {
    var user = GetById(id);
    if (user == null) return false;

    user.Username = username;
    user.Email = email;
    return true;
  }

  public bool Delete(int id) {
    var user = GetById(id);
    return user != null && _users.Remove(user);
  }
}
