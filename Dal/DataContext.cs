﻿using Dal.Configurations;
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
    }
}