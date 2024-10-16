﻿using TestUsers.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data.Models;

namespace TestUsers.Data;

public sealed class DataContext(DbContextOptions<DataContext> _options) : DbContext(_options)
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
    /// Таблица "Сохранённые фильтры пользователей"
    /// </summary>
    public DbSet<UserSaveFilter> UserSaveFilter { get; set; }

    /// <summary>
    /// Таблица "Отношение MtoM сохранение фильтра пользователя и значения параметров категории товаров"
    /// </summary>
    public DbSet<UserSaveFilterRelation> UserSaveFilterRelation { get; set; }

    /// <summary>
    /// Таблица "Сессии пользователей"
    /// </summary>
    public DbSet<UserSession> UserSession { get; set; } 

    /// <summary>
    /// Таблица "Связь многие ко многим между пользователями и ролями"
    /// </summary>
    public DbSet<UserRole> UserRole { get; set; }

    /// <summary>
    /// Таблица "Роль"
    /// </summary>
    public DbSet<Role> Role { get; set; }

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

    /// <summary>
    /// Таблица "Товар"
    /// </summary>
    public DbSet<Product> Product { get; set; }

    /// <summary>
    /// Таблица "Категории для товаров"
    /// </summary>
    public DbSet<ProductCategory> ProductCategory { get; set; }

    /// <summary>
    /// Таблица "Параметры для категорий товаров"
    /// </summary>
    public DbSet<ProductCategoryParameter> ProductCategoryParameter { get; set; }

    /// <summary>
    /// Таблица "Значении параметра для категории товаров"
    /// </summary>
    public DbSet<ProductCategoryParameterValue> ProductCategoryParameterValue { get; set; }

    /// <summary>
    /// Таблица "Выбранные значение параметра для товара"
    /// </summary>
    public DbSet<ProductCategoryParameterValueProduct> ProductCategoryParameterValueProduct { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserContactConfiguration());
        modelBuilder.ApplyConfiguration(new UserLanguageConfiguration());
        modelBuilder.ApplyConfiguration(new UserSaveFilterConfiguration());
        modelBuilder.ApplyConfiguration(new UserSaveFilterRelationConfiguration());
        modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
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