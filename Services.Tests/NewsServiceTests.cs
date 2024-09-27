using AutoMapper;
using Bogus;
using Dal;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using Services.Dtos.News;
using Services.Dtos.Pages;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using System.Linq.Expressions;
using Xunit;

namespace Services.Tests;

public class NewsServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly Mock<INewsTagRepository> _newsTagRepositoryMock;
    private readonly Mock<INewsTagRelationRepository> _newsTagRelationRepositoryMock;
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Faker _faker;
    private readonly INewsService _newsService;
    private readonly DataContext _context;

    public NewsServiceTests()
    {
        _faker = new Faker("ru");
        _mapperMock = new Mock<IMapper>();
        _newsRepositoryMock = new Mock<INewsRepository>();
        _newsTagRepositoryMock = new Mock<INewsTagRepository>();
        _newsTagRelationRepositoryMock = new Mock<INewsTagRelationRepository>();

        _newsService = new NewsService(
            _mapperMock.Object,
            _newsRepositoryMock.Object,
            _newsTagRepositoryMock.Object,
            _newsTagRelationRepositoryMock.Object);
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTests")
            .Options;
        _context = new DataContext(_dbContextOptions);
    }

    /// <summary>
    /// Тестирует получение списка новостей с учётом фильтрации по поисковому запросу и тегам.
    /// </summary>
    [Fact]
    public async Task GetList_ShouldReturnFilteredNewsList_OnValidRequest()
    {
        // Arrange
        var request = new NewsListRequest("Test", 1, new PageRequest(1, 10));

        // Список новостей для возврата из репозитория
        var newsList = new List<News>
        {
            new("Test", _faker.Random.Words(30), DateTime.Now, 1)
            {
                Id = 1,
                Tags = [new NewsTagRelation(1, 1) { NewsTag = new NewsTag("Tag1") { Id = 1 }}]
            },
            new("Other News", _faker.Random.Words(30), DateTime.Now, 1)
            {
                Id = 2,
                Tags = [new NewsTagRelation(2, 2) { NewsTag = new NewsTag("Tag2") { Id = 2 }}]
            }
        };

        // Настройка мока для репозитория
        _newsRepositoryMock
            .Setup(repo => repo.GetListByExpression(It.IsAny<Expression<Func<News, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<News, object>>[]>()))
            .ReturnsAsync(newsList);

        // Мок для маппинга каждой новости в NewsListItem
        _mapperMock
            .Setup(mapper => mapper.Map<News, NewsListItem>(It.IsAny<News>()))
            .Returns((News news) => new NewsListItem(news.Id, news.Title, DateTime.Now, news.AuthorId));

        // Act
        var result = await _newsService.GetList(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items); // Ожидается одна новость, соответствующая фильтру "Test"
        Assert.Equal("Test", result.Items.First().Title);

        // Проверяем, что вызвался метод GetListByExpression один раз
        _newsRepositoryMock.Verify(repo => repo.GetListByExpression(It.IsAny<Expression<Func<News, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<News, object>>[]>()), Times.Once);
    }

    /// <summary>
    /// Тестирует получение детальной информации о новости.
    /// </summary>
    [Fact]
    public async Task GetDetail_ShouldReturnNewsDetail_OnValidNewsId()
    {
        // Arrange
        var newsId = 1;
        var news = new News("Test", _faker.Random.Words(30), DateTime.Now, 1) { Id = newsId };

        _newsRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<News, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);

        _mapperMock
            .Setup(mapper => mapper.Map<News, NewsDetailResponse>(news))
            .Returns(new NewsDetailResponse(1, "Test", "Desc", DateTime.Now, new List<NewsTagResponse>()));

        // Act
        var result = await _newsService.GetDetail(newsId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newsId, result.Id);
        _newsRepositoryMock.Verify(repo => repo.GetByExpression(It.IsAny<Expression<Func<News, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Тестирует создание новой новости с сохранением тегов.
    /// </summary>
    [Fact]
    public async Task Create_ShouldCreateNewsWithTags_OnValidRequest()
    {
        // Arrange
        var request = new NewsCreateRequest("Title", _faker.Random.Words(30), 1, "Tag1, Tag2");
        var news = new News(request.Title, request.Description, DateTime.Now, request.AuthorId);
        var tags = new List<NewsTag> { new("Tag1"), new("Tag2") };

        // Настройка маппера для маппинга строки тегов в объекты NewsTag
        _mapperMock
            .Setup(mapper => mapper.Map<string, List<NewsTag>>(It.IsAny<string>()))
            .Returns(tags);

        // Для первого вызова GetByExpression - возвращаем null (теги ещё не созданы)
        var tagQueue = new Queue<NewsTag?>();
        tagQueue.Enqueue(null); // Первый вызов вернет null для каждого тега
        tagQueue.Enqueue(null);
        tagQueue.Enqueue(new NewsTag("Tag1") { Id = 1 }); // Второй вызов вернет созданный тег Tag1
        tagQueue.Enqueue(new NewsTag("Tag2") { Id = 2 }); // Третий вызов вернет созданный тег Tag2

        // Маппер маппит строку с тегами в список тегов
        _mapperMock
            .Setup(mapper => mapper.Map<string, List<NewsTag>>(It.IsAny<string>()))
            .Returns(tags);

        _mapperMock
            .Setup(mapper => mapper.Map<NewsCreateRequest, News>(It.IsAny<NewsCreateRequest>()))
            .Returns(news);

        _newsTagRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<NewsTag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => tagQueue.Dequeue());

        // Настройка мока для создания тегов
        _newsTagRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<NewsTag>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Настройка мока для сохранения изменений в репозитории тегов
        _newsTagRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Настройка мока для создания новости
        _newsRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<News>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Настройка мока для сохранения изменений в репозитории новостей
        _newsRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Настройка мока для создания связей между новостями и тегами
        _newsTagRelationRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<NewsTagRelation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _newsTagRelationRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _newsService.Create(request);

        // Assert
        _newsTagRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<NewsTag>(), It.IsAny<CancellationToken>()), Times.Exactly(2)); // Два новых тега
        _newsRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<News>(), It.IsAny<CancellationToken>()), Times.Once); // Одна новость
        _newsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Сохранение тегов и новости
        _newsTagRelationRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<NewsTagRelation>(), It.IsAny<CancellationToken>()), Times.Exactly(2)); // Две связи с тегами
        _newsTagRelationRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Сохранение связей
    }

    /// <summary>
    /// Тестирует редактирование новости с обновлением тегов.
    /// </summary>
    [Fact]
    public async Task Edit_ShouldUpdateNewsAndTags_OnValidRequest()
    {
        // Arrange
        var request = new NewsEditRequest(1, "UpdatedTitle", _faker.Random.Words(30), 1, "Tag1, Tag3");
        var news = new News(request.Title, request.Description, DateTime.Now, request.AuthorId) { Id = request.Id };
        var tags = new List<NewsTag> { new("Tag1"), new("Tag3") };

        // Маппер маппит строку с тегами в список тегов
        _mapperMock
            .Setup(mapper => mapper.Map<string, List<NewsTag>>(It.IsAny<string>()))
            .Returns(tags);

        _mapperMock
            .Setup(mapper => mapper.Map<NewsEditRequest, News>(It.IsAny<NewsEditRequest>()))
            .Returns(news);

        // Используем очередь для эмуляции возвращаемого значения при первом и втором вызове
        var tagQueue = new Queue<NewsTag?>([null, null, new NewsTag("Tag1") { Id = 1 }, new NewsTag("Tag3") { Id = 2 }]);

        _newsTagRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<NewsTag, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<NewsTag, object>>[]>()))
            .ReturnsAsync(() => tagQueue.Dequeue()); // Возвращаем элементы из очереди

        _newsTagRepositoryMock
            .Setup(repo =>
                repo.GetListByExpression(It.IsAny<Expression<Func<NewsTag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Репозиторий тегов сохраняет изменения
        _newsTagRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Репозиторий новостей сохраняет изменения
        _newsRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Мок для создания связей между новостями и тегами
        _newsTagRelationRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<NewsTagRelation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _newsTagRelationRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _newsService.Edit(request);

        // Assert
        _newsTagRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<NewsTag>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _newsRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<News>(), It.IsAny<CancellationToken>()), Times.Once);
        _newsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _newsTagRelationRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<NewsTagRelation>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _newsTagRelationRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    /// <summary>
    /// Тестирует удаление новости.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldDeleteNews_OnValidNewsId()
    {
        // Arrange
        var newsId = 1;
        var news = new News("Test", _faker.Random.Words(30), DateTime.Now, 1) { Id = newsId };

        _newsRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<News, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);

        // Act
        await _newsService.Delete(newsId);

        // Assert
        _newsRepositoryMock.Verify(repo => repo.Delete(It.IsAny<News>()), Times.Once);
        _newsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}