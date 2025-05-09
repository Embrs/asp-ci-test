using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TodoItem> Todos { get; set; }  // ✅ 加上這行
}