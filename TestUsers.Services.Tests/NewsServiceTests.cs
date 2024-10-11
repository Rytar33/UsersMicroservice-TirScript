using Bogus;
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
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        var newsList = new List<News>
        {
            new(FakeDataService.GetUniqueName(_faker.Random.Words(3)), FakeDataService.GetUniqueName(_faker.Random.Words(30)), DateTime.UtcNow, user.Id),
            new(FakeDataService.GetUniqueName(_faker.Random.Words(3)), FakeDataService.GetUniqueName(_faker.Random.Words(30)), DateTime.UtcNow, user.Id)
        };
        await _context.News.AddRangeAsync(newsList);
        await _context.SaveChangesAsync();
        var newsTags = new List<NewsTag>
        {
            new(_faker.Random.Word()),
            new(_faker.Random.Word())
        };
        await _context.NewsTag.AddRangeAsync(newsTags);
        await _context.SaveChangesAsync();
        var newsTagRelations = new List<NewsTagRelation>
        {
            new(newsList[0].Id, newsTags[0].Id),
            new(newsList[1].Id, newsTags[1].Id),
        };
        await _context.NewsTagRelation.AddRangeAsync(newsTagRelations);
        await _context.SaveChangesAsync();

        var request = new NewsListRequest(newsList[1].Title, newsTags[1].Id, new PageRequest(1, 10));

        // Act
        var result = await _newsService.GetList(request);

        // Assert
        Assert.DoesNotContain(result.Items, i => i.Title == newsList[0].Title);
        Assert.Contains(result.Items, i => i.Title == newsList[1].Title);
    }

    /// <summary>
    /// Тестирует получение детальной информации о новости.
    /// </summary>
    [Fact]
    public async Task GetDetail_ShouldReturnNewsDetail_OnValidNewsId()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        var news = new News(FakeDataService.GetUniqueName(_faker.Random.Words(3)), FakeDataService.GetUniqueName(_faker.Random.Words(30)), DateTime.UtcNow, user.Id);
        await _context.News.AddAsync(news);
        await _context.SaveChangesAsync();
        var newsTags = new List<NewsTag>
        {
            new(_faker.Random.Word()),
            new(_faker.Random.Word())
        };
        await _context.NewsTag.AddRangeAsync(newsTags);
        await _context.SaveChangesAsync();
        var newsTagRelations = new List<NewsTagRelation>
        {
            new(news.Id, newsTags[0].Id),
            new(news.Id, newsTags[1].Id),
        };
        await _context.NewsTagRelation.AddRangeAsync(newsTagRelations);
        await _context.SaveChangesAsync();

        // Act
        var result = await _newsService.GetDetail(news.Id);

        // Assert
        Assert.Equal(news.Id, result.Id);
        Assert.Equal(newsTags[0].Id, result.Tags[1].Id);
    }

    /// <summary>
    /// Тестирует создание новой новости с сохранением тегов.
    /// </summary>
    [Fact]
    public async Task Create_ShouldCreateNewsWithTags_OnValidRequest()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        var tagsName = new List<string>
        {
            FakeDataService.GetUniqueName("Tag1"),
            FakeDataService.GetUniqueName("Tag2")
        };
        var request = new NewsCreateRequest(FakeDataService.GetUniqueName(_faker.Random.Words(1)), FakeDataService.GetUniqueName(_faker.Random.Words(10)), user.Id, string.Join(", ", tagsName));

        // Act
        await _newsService.Create(request);

        // Assert
        Assert.True(await _context.News.AnyAsync(n => request.Title == n.Title));
        Assert.True(await _context.NewsTag.AnyAsync(nt => nt.Name == tagsName[0]));
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
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        var news = new News(FakeDataService.GetUniqueName("News"), FakeDataService.GetUniqueName(_faker.Random.Words(10)), DateTime.UtcNow, user.Id);
        await _context.News.AddAsync(news);
        await _context.SaveChangesAsync();
        var newsTags = new List<NewsTag>
        {
            new(FakeDataService.GetUniqueName("Tag1")), new(FakeDataService.GetUniqueName("Tag2"))
        };
        await _context.NewsTag.AddRangeAsync(newsTags);
        await _context.SaveChangesAsync();
        var newsTagRelations = new List<NewsTagRelation>
        {
            new(news.Id, newsTags[0].Id),
            new(news.Id, newsTags[1].Id),
        };
        await _context.NewsTagRelation.AddRangeAsync(newsTagRelations);
        await _context.SaveChangesAsync();
        var newTag = FakeDataService.GetUniqueName("Tag3");
        var request = new NewsEditRequest(news.Id, FakeDataService.GetUniqueName("News"), FakeDataService.GetUniqueName(_faker.Random.Words(10)), user.Id, string.Join(", ", newsTags[0].Name, newTag));

        // Act
        await _newsService.Edit(request);

        // Assert
        Assert.NotEqual(news.Title, request.Title);
        Assert.NotEqual(news.Description, request.Description);
        Assert.False(await _context.NewsTag.AnyAsync(nt => nt.Name == newsTags[1].Name));
        Assert.False(await _context.NewsTagRelation.AnyAsync(nt => nt.Id == newsTagRelations[1].Id));
        Assert.True(await _context.NewsTag.AnyAsync(nt => nt.Name == newTag));
    }

    /// <summary>
    /// Тестирует удаление новости.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldDeleteNews_OnValidNewsId()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        var news = new News(FakeDataService.GetUniqueName(_faker.Random.Words(3)), FakeDataService.GetUniqueName(_faker.Random.Words(30)), DateTime.UtcNow, user.Id);
        await _context.News.AddAsync(news);
        await _context.SaveChangesAsync();

        // Act
        await _newsService.Delete(news.Id);

        // Assert
        Assert.False(await _context.News.AnyAsync(n => n.Id == news.Id));
    }

}