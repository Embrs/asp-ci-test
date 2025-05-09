using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options){
  public DbSet<TodoItem> Todos => Set<TodoItem>();
}
