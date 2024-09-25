using AutoMapper;
using Bogus;
using Dal;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using Services.Dtos.UserLanguages;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using System.Linq.Expressions;
using Xunit;

namespace Services.Tests;

/// <summary>
/// Юнит тесты для сервиса языков пользователей
/// </summary>
public class UserLanguageServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserLanguageRepository> _userLanguageRepositoryMock;
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Faker _faker;
    private readonly IUserLanguageService _userLanguageService;

    public UserLanguageServiceTests()
    {
        _faker = new Faker("ru");
        _mapperMock = new Mock<IMapper>();
        _userLanguageRepositoryMock = new Mock<IUserLanguageRepository>();

        _userLanguageService = new UserLanguageService(_mapperMock.Object, _userLanguageRepositoryMock.Object);
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTests")
            .Options;
    }

    /// <summary>
    /// Проверяет, возвращает ли сервис список языков пользователя
    /// </summary>
    [Fact]
    public async Task GetList_ShouldReturnMappedLanguages_WhenUserHasLanguages()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var userLanguages = new List<UserLanguage>
        {
            new(DateTime.Now, userId, 1) { Language = new Language("EN", "English") }
        };

        _userLanguageRepositoryMock
            .Setup(repo => repo.GetListByExpression(It.IsAny<Expression<Func<UserLanguage, bool>>>(),
                It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<UserLanguage, object>>>()))
            .ReturnsAsync(userLanguages);

        _mapperMock
            .Setup(mapper => mapper.Map<UserLanguage, UserLanguageItemResponse>(It.IsAny<UserLanguage>()))
            .Returns(new UserLanguageItemResponse(1, "EN", "English", userLanguages[0].DateLearn));

        // Act
        var result = await _userLanguageService.GetList(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("English", result[0].Name);
        Assert.Equal("EN", result[0].Code);
    }

    /// <summary>
    /// Проверяет, добавляется ли язык пользователю
    /// </summary>
    [Fact]
    public async Task AddLanguageToUser_ShouldAddLanguage_WhenValidRequestIsPassed()
    {
        // Arrange
        var request = new AddLanguageToUser(_faker.Random.Int(1, 100), _faker.Random.Int(1, 100), DateTime.UtcNow);
        var userLanguage = new UserLanguage(request.DateLearn, request.UserId, request.LanguageId);

        _mapperMock
            .Setup(mapper => mapper.Map<AddLanguageToUser, UserLanguage>(request))
            .Returns(userLanguage);

        // Act
        await _userLanguageService.AddLanguageToUser(request);

        // Assert
        _userLanguageRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<UserLanguage>(), It.IsAny<CancellationToken>()), Times.Once);
        _userLanguageRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Проверяет сохранение списка языков пользователя и правильную обработку транзакций
    /// </summary>
    [Fact]
    public async Task SaveUserLanguages_ShouldSaveNewLanguagesAndDeleteOld_OnValidRequest()
    {
        // Arrange
        var dateLearn = DateTime.Now;
        var userId = _faker.Random.Int(1, 100);
        var request = new SaveUserLanguagesRequest(userId, [new SaveUserLanguageItem(1, dateLearn)]);

        var existingLanguages = new List<UserLanguage>
        {
            new(dateLearn, userId, 2)
        };

        _userLanguageRepositoryMock
            .Setup(repo => repo.GetListByExpression(It.IsAny<Expression<Func<UserLanguage, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLanguages);

        _mapperMock
            .Setup(mapper => mapper.Map<SaveUserLanguagesRequest, List<UserLanguage>>(request))
            .Returns([
                new UserLanguage(dateLearn, userId, 1)
            ]);

        // Act
        await _userLanguageService.SaveUserLanguages(request);

        // Assert
        _userLanguageRepositoryMock.Verify(repo => repo.Delete(It.Is<UserLanguage>(ul => ul.LanguageId == 2)), Times.Once);
        _userLanguageRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<UserLanguage>(), It.IsAny<CancellationToken>()), Times.Once);
        _userLanguageRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    /// <summary>
    /// Проверяет откат транзакции при возникновении исключения во время сохранения языков пользователя
    /// </summary>
    [Fact]
    public async Task SaveUserLanguages_ShouldRollbackTransaction_WhenExceptionThrown()
    {
        // Arrange
        var request = new SaveUserLanguagesRequest(_faker.Random.Int(1, 100), new List<SaveUserLanguageItem>());

        _userLanguageRepositoryMock
            .Setup(repo => repo.StartTransaction(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Transaction error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userLanguageService.SaveUserLanguages(request));

        _userLanguageRepositoryMock.Verify(repo => repo.RollBackTransaction(It.IsAny<CancellationToken>()), Times.Once);
    }
}
