using MyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyApp;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserCredential> UserCredentials => Set<UserCredential>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.PublicId)
            .IsUnique();

        modelBuilder.Entity<UserCredential>()
            .HasIndex(c => c.PublicId)
            .IsUnique();

        modelBuilder.Entity<UserCredential>()
            .HasIndex(c => new { c.Provider, c.Identifier })
            .IsUnique();
    }
}
