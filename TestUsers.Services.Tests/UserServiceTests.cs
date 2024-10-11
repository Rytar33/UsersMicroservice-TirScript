using Xunit;
using TestUsers.Services.Dtos.Users;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.Pages;
using TestUsers.Services.Exceptions;
using TestUsers.Services.Dtos.Users.Recoveries;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data;
using TestUsers.Services.Extensions;
using TestUsers.Services.Interfaces.Services;
using Bogus;
using TestUsers.WebApi.Options;



namespace TestUsers.Services.Tests;

/// <summary>
/// Тесты для сервисов пользователя
/// </summary>
public class UserServiceTests
{
    // Поля для моков и других зависимостей
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Faker _faker;
    private readonly DataContext _context;

    public UserServiceTests()
    {
        // Инициализация данных для тестов
        _faker = new Faker("ru");
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: nameof(UserServiceTests))
            .Options;
        _context = new DataContext(_dbContextOptions);
        // Настройка мока IEmailOptions с тестовыми данными
        var emailOptions = new EmailOptions();
        _emailService = new EmailService(emailOptions, _context);
        _userService = new UserService(_emailService, _context);
    }

    /// <summary>
    /// Тестирует метод GetList.
    /// Ожидает, что метод вернет правильные данные при корректном запросе.
    /// </summary>
    [Fact]
    public async Task GetList_ShouldReturnCorrectData()
    {
        // Arrange
        var users = new List<User>
        {
            FakeDataService.GetGenerationUser(),
            FakeDataService.GetGenerationUser()
        };

        var request = new UsersListRequest(users[1].FullName, null, new PageRequest());

        await _context.User.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.GetList(request);

        // Assert
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
        var user = FakeDataService.GetGenerationUser();
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.GetDetail(user.Id);

        // Assert
        Assert.Equal(user.Email, result.Email);
    }

    /// <summary>
    /// Тестирует метод GetDetail для несуществующего пользователя.
    /// Ожидает, что метод выбросит исключение NotFoundException.
    /// </summary>
    [Fact]
    public async Task GetDetail_UserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = 10;

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
        var request = new UserCreateRequest(_faker.Internet.Email(), _faker.Person.FullName, _faker.Internet.Password());

        // Act
        await _userService.Create(request);

        // Assert
        Assert.True(await _context.User.AnyAsync(u => u.Email == request.Email));
    }

    /// <summary>
    /// Тестирует метод Edit для существующего пользователя.
    /// Ожидает, что метод обновит пользователя и вызовет необходимые методы репозитория.
    /// </summary>
    [Fact]
    public async Task Edit_UserExists_ShouldUpdateUser()
    {
        // Arrange
        var existingUser = FakeDataService.GetGenerationUser();
        await _context.User.AddAsync(existingUser);
        await _context.SaveChangesAsync();

        var request = new UserEditRequest(existingUser.Id, _faker.Name.FullName());

        // Act
        await _userService.Edit(request);

        // Assert
        Assert.True(await _context.User.AnyAsync(u => u.FullName == request.FullName));
    }

    /// <summary>
    /// Тестирует метод Edit для несуществующего пользователя.
    /// Ожидает, что метод выбросит исключение NotFoundException.
    /// </summary>
    [Fact]
    public async Task Edit_UserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new UserEditRequest(1, _faker.Name.FullName());

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
        var existingUser = FakeDataService.GetGenerationUser();
        await _context.User.AddAsync(existingUser);
        await _context.SaveChangesAsync();

        // Act
        await _userService.Delete(existingUser.Id);

        // Assert
        Assert.False(await _context.User.AnyAsync(u => existingUser.Id == u.Id));
    }

    /// <summary>
    /// Тестирует метод Delete для несуществующего пользователя.
    /// Ожидает, что метод выбросит исключение NotFoundException.
    /// </summary>
    [Fact]
    public async Task Delete_UserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = 10;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.Delete(userId));
    }

    /// <summary>
    /// Тестирует метод RecoveryStart для существующего пользователя.
    /// Ожидает, что пользователю отправится код с подтверждением
    /// </summary>
    [Fact]
    public async Task RecoveryStart_ShouldGenerateTokenAndSendEmail_WhenRequestCodeIsNull()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        var request = new RecoveryStartRequest(user.Email, null);

        // Act
        await _userService.RecoveryStart(request);

        // Assert

        var userFound = await _context.User.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(userFound?.RecoveryToken); // Токен должен быть сгенерирован
    }

    /// <summary>
    /// Тестирует метод RecoveryStart для несуществующего пользователя.
    /// Ожидает, что метод выбросит исключение NotFoundException.
    /// </summary>
    [Fact]
    public async Task RecoveryStart_UserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new RecoveryStartRequest(_faker.Internet.Email(), null);

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
        var user = FakeDataService.GetGenerationUser();
        user.RecoveryToken = string.Empty.GetGenerateToken();
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        var newPassword = _faker.Internet.Password();
        var request = new RecoveryEndRequest(user.Email, user.RecoveryToken, newPassword, newPassword);

        // Act
        await _userService.RecoveryEnd(request);

        // Assert
        Assert.Null(user.RecoveryToken); // Токен должен быть очищен после смены пароля
        Assert.Equal(newPassword.GetSha256(), user.PasswordHash);
    }

    /// <summary>
    /// Тестирует метод RecoveryEnd для не верного токена.
    /// Ожидает, что метод выбросит исключение ArgumentException.
    /// </summary>
    [Fact]
    public async Task RecoveryEnd_ShouldThrowException_WhenInvalidTokenProvided()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        user.RecoveryToken = string.Empty.GetGenerateToken();
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();

        var request = new RecoveryEndRequest(user.Email, string.Empty.GetGenerateToken(), _faker.Internet.Password(), _faker.Internet.Password());

        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userService.RecoveryEnd(request));
    }
}
