using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TodoItem> todos { get; set; }  // ✅ 要定義 DbSet
}
