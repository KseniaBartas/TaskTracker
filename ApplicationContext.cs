namespace TaskTrecker;

using Microsoft.EntityFrameworkCore;
using TaskTrecker.Models;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}
