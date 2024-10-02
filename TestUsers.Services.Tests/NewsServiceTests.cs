﻿using Bogus;
using TestUsers.Data;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.News;
using TestUsers.Services.Dtos.Pages;
using TestUsers.Services.Interfaces.Services;
using Xunit;

namespace TestUsers.Services.Tests;

public class NewsServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Faker _faker;
    private readonly INewsService _newsService;
    private readonly DataContext _context;

    public NewsServiceTests()
    {
        _faker = new Faker("ru");
        
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: nameof(NewsServiceTests))
            .Options;
        _context = new DataContext(_dbContextOptions);
        _newsService = new NewsService(_context);
    }

    /// <summary>
    /// Тестирует получение списка новостей с учётом фильтрации по поисковому запросу и тегам.
    /// </summary>
    [Fact]
    public async Task GetList_ShouldReturnFilteredNewsList_OnValidRequest()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await using var db = new DataContext(_dbContextOptions);
        await db.User.AddAsync(user);
        await db.SaveChangesAsync();
        var newsList = new List<News>
        {
            new(_faker.Random.Words(3), _faker.Random.Words(30), DateTime.UtcNow, user.Id),
            new(_faker.Random.Words(3), _faker.Random.Words(30), DateTime.UtcNow, user.Id)
        };
        await db.News.AddRangeAsync(newsList);
        await db.SaveChangesAsync();
        var newsTags = new List<NewsTag>
        {
            new(_faker.Random.Word()),
            new(_faker.Random.Word())
        };
        await db.NewsTag.AddRangeAsync(newsTags);
        await db.SaveChangesAsync();
        var newsTagRelations = new List<NewsTagRelation>
        {
            new(newsList[0].Id, newsTags[0].Id),
            new(newsList[1].Id, newsTags[1].Id),
        };
        await db.NewsTagRelation.AddRangeAsync(newsTagRelations);
        await db.SaveChangesAsync();

        var request = new NewsListRequest(newsList[1].Title, newsTags[1].Id, new PageRequest(1, 10));

        // Act
        var result = await _newsService.GetList(request);

        // Assert
        Assert.Single(result.Items); // Ожидается одна новость, соответствующая фильтру "Test"
        Assert.Equal(newsList[1].Title, result.Items.First().Title);
    }

    /// <summary>
    /// Тестирует получение детальной информации о новости.
    /// </summary>
    [Fact]
    public async Task GetDetail_ShouldReturnNewsDetail_OnValidNewsId()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await using var db = new DataContext(_dbContextOptions);
        await db.User.AddAsync(user);
        await db.SaveChangesAsync();
        var news = new News(_faker.Random.Words(3), _faker.Random.Words(30), DateTime.UtcNow, user.Id);
        await db.News.AddAsync(news);
        await db.SaveChangesAsync();
        var newsTags = new List<NewsTag>
        {
            new(_faker.Random.Word()),
            new(_faker.Random.Word())
        };
        await db.NewsTag.AddRangeAsync(newsTags);
        await db.SaveChangesAsync();
        var newsTagRelations = new List<NewsTagRelation>
        {
            new(news.Id, newsTags[0].Id),
            new(news.Id, newsTags[1].Id),
        };
        await db.NewsTagRelation.AddRangeAsync(newsTagRelations);
        await db.SaveChangesAsync();

        // Act
        var result = await _newsService.GetDetail(news.Id);

        // Assert
        Assert.Equal(news.Id, result.Id);
        Assert.Equal(newsTags[1].Id, result.Tags[1].Id);
    }

    /// <summary>
    /// Тестирует создание новой новости с сохранением тегов.
    /// </summary>
    [Fact]
    public async Task Create_ShouldCreateNewsWithTags_OnValidRequest()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await using var db = new DataContext(_dbContextOptions);
        await db.User.AddAsync(user);
        await db.SaveChangesAsync();
        var request = new NewsCreateRequest(_faker.Random.Words(3), _faker.Random.Words(30), user.Id, "Tag1, Tag2");

        // Act
        await _newsService.Create(request);

        // Assert
        Assert.True(await db.News.AnyAsync(n => request.Title == n.Title));
        Assert.True(await db.NewsTag.AnyAsync(nt => nt.Name == "Tag1"));
    }

    /// <summary>
    /// Тестирует редактирование новости с обновлением тегов.
    /// </summary>
    [Fact]
    public async Task Edit_ShouldUpdateNewsAndTags_OnValidRequest()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await using var db = new DataContext(_dbContextOptions);
        await db.User.AddAsync(user);
        await db.SaveChangesAsync();
        var news = new News(_faker.Random.Words(3), _faker.Random.Words(30), DateTime.UtcNow, user.Id);
        await db.News.AddAsync(news);
        await db.SaveChangesAsync();
        var newsTags = new List<NewsTag>
        {
            new("Tag1"), new("Tag2")
        };
        await db.NewsTag.AddRangeAsync(newsTags);
        await db.SaveChangesAsync();
        var newsTagRelations = new List<NewsTagRelation>
        {
            new(news.Id, newsTags[0].Id),
            new(news.Id, newsTags[1].Id),
        };
        await db.NewsTagRelation.AddRangeAsync(newsTagRelations);
        await db.SaveChangesAsync();
        var request = new NewsEditRequest(news.Id, _faker.Random.Words(3), _faker.Random.Words(30), user.Id, "Tag1, Tag3");

        // Act
        await _newsService.Edit(request);

        // Assert
        Assert.Equal(news.Title, request.Title);
        Assert.Equal(news.Description, request.Description);
        Assert.False(await db.NewsTag.AnyAsync(nt => nt.Id == newsTags[1].Id));
        Assert.False(await db.NewsTagRelation.AnyAsync(nt => nt.Id == newsTagRelations[1].Id));
        Assert.True(await db.NewsTag.AnyAsync(nt => nt.Name == "Tag3"));
    }

    /// <summary>
    /// Тестирует удаление новости.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldDeleteNews_OnValidNewsId()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await using var db = new DataContext(_dbContextOptions);
        await db.User.AddAsync(user);
        await db.SaveChangesAsync();
        var news = new News(_faker.Random.Words(3), _faker.Random.Words(30), DateTime.UtcNow, user.Id);
        await db.News.AddAsync(news);
        await db.SaveChangesAsync();

        // Act
        await _newsService.Delete(news.Id);

        // Assert
        Assert.False(await db.News.AnyAsync(n => n.Id == news.Id));
    }

}