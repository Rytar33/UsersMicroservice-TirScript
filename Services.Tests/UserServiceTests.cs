using System.Linq.Expressions;
using Xunit;
using Moq;
using AutoMapper;
using Services.Dtos.Users;
using Models;
using Services.Dtos.Pages;
using Models.Exceptions;
using Services.Dtos.Users.Recoveries;
using Microsoft.EntityFrameworkCore;
using Dal;
using Models.Extensions;
using Services.Interfaces.Repositories;
using Microsoft.Extensions.Options;
using Services.Interfaces.Options;
using Services.Options;
using Services.Interfaces.Services;
using Bogus;

namespace Services.Tests;

/// <summary>
/// Тесты для сервисов пользователя
/// </summary>
public class UserServiceTests
{
    // Поля для моков и других зависимостей
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IOptions<IEmailOptions>> _emailOptionsMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly UserService _userService;
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Faker _faker;
    public UserServiceTests()
    {
        // Инициализация данных для тестов
        _faker = new Faker("ru");
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        // Mock IOptions<IEmailOptions>
        _emailOptionsMock = new Mock<IOptions<IEmailOptions>>();

        // Настройка мока IEmailOptions с тестовыми данными
        var emailOptions = new EmailOptions(
            "TestCompany",
            "test@company.com",
            "testpassword",
            "smtp.mail.ru",
            "587",
            "localhost");
        _emailOptionsMock.Setup(o => o.Value).Returns(emailOptions);

        _emailServiceMock = new Mock<IEmailService>();
        _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object, _emailServiceMock.Object);

        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTests")
            .Options;
    }

    /// <summary>
    /// Тестирует метод GetList.
    /// Ожидает, что метод вернет правильные данные при корректном запросе.
    /// </summary>
    [Fact]
    public async Task GetList_ShouldReturnCorrectData()
    {
        // Arrange
        await using var context = new DataContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();

        var users = new List<User>
        {
            FakeDataService.GetGenerationUser(),
            FakeDataService.GetGenerationUser()
        };

        var request = new UsersListRequest(users[1].FullName, null, new PageRequest());

        await context.User.AddRangeAsync(users);
        await context.SaveChangesAsync();

        _userRepositoryMock.Setup(repo => repo.GetListByExpression(null, It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mapperMock
            .Setup(mapper => mapper.Map<User, UsersListItem>(It.IsAny<User>()))
            .Returns((User u) => new UsersListItem(u.Id, u.Email, u.FullName, u.DateRegister, u.Status));

        // Act
        var result = await _userService.GetList(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Items);
        Assert.Equal(users[1].FullName, result.Items[0].FullName);
    }

    /// <summary>
    /// Тестирует метод GetDetail для существующего пользователя.
    /// Ожидает, что метод вернет детали пользователя.
    /// </summary>
    [Fact]
    public async Task GetDetail_UserExists_ShouldReturnUserDetails()
    {
        // Arrange
        var userId = 1;
        var user = FakeDataService.GetGenerationUser();
        user.Id = userId; // Assigning a user ID for the test

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(mapper => mapper.Map<User, UserDetailResponse>(user))
            .Returns(new UserDetailResponse(user.Id, user.Email, user.FullName, user.DateRegister, user.Status));

        // Act
        var result = await _userService.GetDetail(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.FullName, result.FullName);
    }

    /// <summary>
    /// Тестирует метод GetDetail для несуществующего пользователя.
    /// Ожидает, что метод выбросит исключение NotFoundException.
    /// </summary>
    [Fact]
    public async Task GetDetail_UserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = 1;

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetDetail(userId));
    }

    /// <summary>
    /// Тестирует метод Create с валидным запросом.
    /// Ожидает, что метод создаст пользователя и вызовет необходимые методы репозитория.
    /// </summary>
    [Fact]
    public async Task Create_ValidRequest_ShouldCreateUser()
    {
        // Arrange
        var request = new UserCreateRequest(_faker.Internet.Email(), _faker.Person.FullName, "qwerty123");
        var user = new User(request.Email, request.FullName, "qwerty123".GetSha256());

        _mapperMock.Setup(mapper => mapper.Map<UserCreateRequest, User>(request)).Returns(user);
        _userRepositoryMock.Setup(repo => repo.CreateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _userService.Create(request);

        // Assert
        Assert.True(result.IsSuccess);
        _userRepositoryMock.Verify(repo => repo.CreateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Тестирует метод Edit для существующего пользователя.
    /// Ожидает, что метод обновит пользователя и вызовет необходимые методы репозитория.
    /// </summary>
    [Fact]
    public async Task Edit_UserExists_ShouldUpdateUser()
    {
        // Arrange
        var request = new UserEditRequest(1, "Updated User");

        var existingUser = FakeDataService.GetGenerationUser();
        existingUser.Id = request.Id; // Assigning a user ID for the test

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(repo => repo.UpdateAsync(existingUser, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _userService.Edit(request);

        // Assert
        Assert.True(result.IsSuccess);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(existingUser, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Updated User", existingUser.FullName);
    }

    /// <summary>
    /// Тестирует метод Edit для несуществующего пользователя.
    /// Ожидает, что метод выбросит исключение NotFoundException.
    /// </summary>
    [Fact]
    public async Task Edit_UserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new UserEditRequest(1, "Updated User");

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.Edit(request));
    }

    /// <summary>
    /// Тестирует метод Delete для существующего пользователя.
    /// Ожидает, что метод удалит пользователя и вызовет необходимые методы репозитория.
    /// </summary>
    [Fact]
    public async Task Delete_UserExists_ShouldDeleteUser()
    {
        // Arrange
        var userId = 1;
        var existingUser = FakeDataService.GetGenerationUser();
        existingUser.Id = userId; // Assigning a user ID for the test

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(repo => repo.Delete(existingUser)).Verifiable();
        _userRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _userService.Delete(userId);

        // Assert
        Assert.True(result.IsSuccess);
        _userRepositoryMock.Verify(repo => repo.Delete(existingUser), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Тестирует метод Delete для несуществующего пользователя.
    /// Ожидает, что метод выбросит исключение NotFoundException.
    /// </summary>
    [Fact]
    public async Task Delete_UserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = 1;

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.Delete(userId));
    }

    /// <summary>
    /// Тестирует метод RecoveryStart для существующего пользователя.
    /// Ожидает, что метод вызовет SendIntoEmail и вернет результат, а так же что пользователю отправится код с подтверждением
    /// </summary>
    [Fact]
    public async Task RecoveryStart_ShouldGenerateTokenAndSendEmail_WhenRequestCodeIsNull()
    {
        // Arrange
        var request = new RecoveryStartRequest("test@test.com", null);
        var user = FakeDataService.GetGenerationUser();

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _userService.RecoveryStart(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(user.RecoveryToken); // Токен должен быть сгенерирован
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _emailServiceMock.Verify(email => email.SendEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Тестирует метод RecoveryStart для существующего пользователя.
    /// Ожидает, что метод вызовет SendIntoEmail и вернет результат.
    /// </summary>
    [Fact]
    public async Task RecoveryStart_ShouldNotSendEmail_WhenRequestCodeIsProvided()
    {
        // Arrange
        var request = new RecoveryStartRequest("test@test.com", "123456");
        var user = FakeDataService.GetGenerationUser();
        user.RecoveryToken = "123456";

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userService.RecoveryStart(request);

        // Assert
        _emailServiceMock.Verify(email => email.SendEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Тестирует метод RecoveryStart для несуществующего пользователя.
    /// Ожидает, что метод выбросит исключение NotFoundException.
    /// </summary>
    [Fact]
    public async Task RecoveryStart_UserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new RecoveryStartRequest("notfound@user.com", null);
        _userRepositoryMock.Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.RecoveryStart(request));
    }

    /// <summary>
    /// Тестирует метод RecoveryEnd для восстановления пароля.
    /// Ожидает, что метод обновит пароль и вызовет необходимые методы репозитория.
    /// </summary>
    [Fact]
    public async Task RecoveryEnd_ShouldCompleteRecovery_WhenValidTokenProvided()
    {
        // Arrange
        var request = new RecoveryEndRequest("test@test.com", "123456", "newpassword", "newpassword");
        var user = FakeDataService.GetGenerationUser();
        user.RecoveryToken = "123456";

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _userService.RecoveryEnd(request);

        // Assert
        Assert.Null(user.RecoveryToken); // Токен должен быть очищен после смены пароля
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Тестирует метод RecoveryEnd для не верного токена.
    /// Ожидает, что метод выбросит исключение ArgumentException.
    /// </summary>
    [Fact]
    public async Task RecoveryEnd_ShouldThrowException_WhenInvalidTokenProvided()
    {
        // Arrange
        var request = new RecoveryEndRequest("test@test.com", "654321", "newpassword", "newpassword");
        var user = FakeDataService.GetGenerationUser();
        user.RecoveryToken = "123456";

        _userRepositoryMock
            .Setup(repo => repo.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.RecoveryEnd(request));
    }
}
