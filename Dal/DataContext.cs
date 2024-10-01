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

    /// <summary>
    /// Таблица "Контакт пользователя"
    /// </summary>
    public DbSet<UserContact> UserContact { get; set; }

    /// <summary>
    /// Таблица "Язык пользователя"
    /// </summary>
    public DbSet<UserLanguage> UserLanguage { get; set; }

    /// <summary>
    /// Таблица "Язык"
    /// </summary>
    public DbSet<Language> Language { get; set; }

    /// <summary>
    /// Таблица "Новости"
    /// </summary>
    public DbSet<News> News { get; set; }

    /// <summary>
    /// Таблица "Теги"
    /// </summary>
    public DbSet<NewsTag> NewsTag { get; set; }

    /// <summary>
    /// Таблица "Связь новостей с тегами"
    /// </summary>
    public DbSet<NewsTagRelation> NewsTagRelation { get; set; }

    public DbSet<Product> Product { get; set; }

    public DbSet<ProductCategory> ProductCategory { get; set; }

    public DbSet<ProductCategoryParameter> ProductCategoryParameter { get; set; }

    public DbSet<ProductCategoryParameterValue> ProductCategoryParameterValue { get; set; }

    public DbSet<ProductCategoryParameterValueProduct> ProductCategoryParameterValueProduct { get; set; }

    public DataContext() {}

    public DataContext(DbContextOptions<DataContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer("Server=.;Database=Pages;Trusted_Connection=True;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfigurations());
        modelBuilder.ApplyConfiguration(new UserContactConfiguration());
        modelBuilder.ApplyConfiguration(new UserLanguageConfiguration());
        modelBuilder.ApplyConfiguration(new LanguageConfiguration());
        modelBuilder.ApplyConfiguration(new NewsConfiguration());
        modelBuilder.ApplyConfiguration(new NewsTagConfiguration());
        modelBuilder.ApplyConfiguration(new NewsTagRelationConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCategoryParameterConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCategoryParameterValueConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCategoryParameterValueProductConfiguration());
    }
}