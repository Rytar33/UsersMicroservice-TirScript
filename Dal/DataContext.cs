using Dal.Configurations;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal;

public sealed class DataContext : DbContext
{
    /// <summary>
    /// Таблица "Пользователь"
    /// </summary>
    public DbSet<User> User { get; set; }

    public DataContext() {}

    public DataContext(DbContextOptions options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer("Server=.;Database=Pages;Trusted_Connection=True;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfigurations());
    }
}