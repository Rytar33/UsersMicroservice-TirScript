using AutoMapper;
using Bogus;
using Dal;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using Services.Dtos.UserContacts;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using System.Linq.Expressions;
using Models.Exceptions;
using Models.Extensions;
using Xunit;

namespace Services.Tests;

/// <summary>
/// Юнит тесты для сервиса контактов пользователя
/// </summary>
public class UserContactServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserContactRepository> _userContactRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Faker _faker;
    private readonly IUserContactService _userContactService;
    public UserContactServiceTests()
    {
        _faker = new Faker("ru");
        _mapperMock = new Mock<IMapper>();
        _userContactRepositoryMock = new Mock<IUserContactRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _userContactService = new UserContactService(_mapperMock.Object, _userContactRepositoryMock.Object, _userRepositoryMock.Object);
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTests")
            .Options;
    }

    /// <summary>
    /// Тест проверяет, возвращаются ли контакты пользователя в случае, если пользователь существует
    /// </summary>
    [Fact]
    public async Task GetContacts_ShouldReturnMappedContacts_WhenUserExists()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var user = new User(
            _faker.Internet.Email(),
            _faker.Name.FullName(),
            _faker.Internet.Password().GetSha256())
        {
            Id = userId,
            Contacts = 
                [
                    new UserContact("Test", "Value", userId)
                ]
        };
        _userRepositoryMock
            .Setup(x => x.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(m => m.Map<UserContact, UserContactItem>(It.IsAny<UserContact>()))
            .Returns(new UserContactItem(userId, "Test", "Value"));

        // Act
        var result = await _userContactService.GetContacts(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test", result[0].Name);
        Assert.Equal("Value", result[0].Value);
    }

    /// <summary>
    /// Тест проверяет, выбрасывается ли исключение, если пользователь не найден
    /// </summary>
    [Fact]
    public async Task GetContacts_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        _userRepositoryMock
            .Setup(x => x.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(null as User);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userContactService.GetContacts(userId));
    }

    /// <summary>
    /// Тест проверяет, создаются ли новые контакты пользователя, если они не существовали ранее
    /// </summary>
    [Fact]
    public async Task SaveContacts_ShouldCreateNewContacts_WhenContactsAreNew()
    {
        // Arrange
        var request = new UserContactsSaveRequest(_faker.Random.Int(1, 100), [new UserContactItem(null, "New Contact", "Value")]);

        _userContactRepositoryMock
            .Setup(x => x.GetListByExpression(It.IsAny<Expression<Func<UserContact, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _userContactService.SaveContacts(request);

        // Assert
        _userContactRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserContact>(), It.IsAny<CancellationToken>()), Times.Once);
        _userContactRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Тест проверяет, удаляются ли старые контакты пользователя, если их нет в новом списке
    /// </summary>
    [Fact]
    public async Task SaveContacts_ShouldDeleteOldContacts_WhenNotPresentInNewList()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var request = new UserContactsSaveRequest(userId, [new UserContactItem(2, "Updated Contact", "Value")]);

        var existingContacts = new List<UserContact>
        {
            new("Old Contact", "OldValue", userId) { Id = 1 }
        };

        _userContactRepositoryMock
            .Setup(x => x.GetListByExpression(It.IsAny<Expression<Func<UserContact, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingContacts);

        // Act
        await _userContactService.SaveContacts(request);

        // Assert
        _userContactRepositoryMock.Verify(x => x.Delete(It.Is<UserContact>(uc => uc.Id == 1)), Times.Once);
    }

    /// <summary>
    /// Тест проверяет, откатывается ли транзакция при возникновении исключения
    /// </summary>
    [Fact]
    public async Task SaveContacts_ShouldRollbackTransaction_WhenExceptionThrown()
    {
        // Arrange
        var request = new UserContactsSaveRequest(_faker.Random.Int(1, 100), []);

        _userContactRepositoryMock
            .Setup(x => x.StartTransaction(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Transaction error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userContactService.SaveContacts(request));

        _userContactRepositoryMock.Verify(x => x.RollBackTransaction(It.IsAny<CancellationToken>()), Times.Once);
    }
}