using Bogus;
using TestUsers.Data;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.UserLanguages;
using TestUsers.Services.Interfaces.Services;
using Xunit;

namespace TestUsers.Services.Tests;

/// <summary>
/// Юнит тесты для сервиса языков пользователей
/// </summary>
public class UserLanguageServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly DataContext _dbContext;
    private readonly Faker _faker;
    private readonly IUserLanguageService _userLanguageService;

    public UserLanguageServiceTests()
    {
        _faker = new Faker("ru");
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: nameof(UserLanguageServiceTests))
            .Options;
        _dbContext = new DataContext(_dbContextOptions);
        _userLanguageService = new UserLanguageService(_dbContext);
    }

    /// <summary>
    /// Проверяет, возвращает ли сервис список языков пользователя
    /// </summary>
    [Fact]
    public async Task GetList_ShouldReturnMappedLanguages_WhenUserHasLanguages()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        var languages = new List<Language> { new("en", "English"), new("ro", "Romanian") };
        await using var db = new DataContext(_dbContextOptions);
        await db.User.AddAsync(user);
        await db.Language.AddRangeAsync(languages);
        await db.SaveChangesAsync();
        var userLanguages = new List<UserLanguage>
        {
            new(DateTime.UtcNow, user.Id, languages[0].Id),
            new(DateTime.UtcNow, user.Id, languages[1].Id)
        };
        await db.UserLanguage.AddRangeAsync(userLanguages);
        await db.SaveChangesAsync();

        // Act
        var result = await _userLanguageService.GetList(user.Id);

        // Assert
        Assert.Equal(result.Count, userLanguages.Count);
        Assert.Equal(languages[0].Name, result[0].Name);
        Assert.Equal(languages[1].Code, result[1].Code);
    }

    /// <summary>
    /// Проверяет, добавляется ли язык пользователю
    /// </summary>
    [Fact]
    public async Task AddLanguageToUser_ShouldAddLanguage_WhenValidRequestIsPassed()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        var language = new Language("en", "English");
        await using var db = new DataContext(_dbContextOptions);
        await db.User.AddAsync(user);
        await db.Language.AddAsync(language);
        await db.SaveChangesAsync();
        var request = new AddLanguageToUser(user.Id, language.Id, DateTime.UtcNow);

        // Act
        await _userLanguageService.AddLanguageToUser(request);

        // Assert
        Assert.True(await db.UserLanguage.AnyAsync(ul => ul.LanguageId == language.Id && ul.UserId == user.Id));
    }

    /// <summary>
    /// Проверяет сохранение списка языков пользователя
    /// </summary>
    [Fact]
    public async Task SaveUserLanguages_ShouldSaveNewLanguagesAndDeleteOld_OnValidRequest()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        var languages = new List<Language> { new("en", "English"), new("ro", "Romanian"), new("ru", "Russian") };
        await using var db = new DataContext(_dbContextOptions);
        await db.User.AddAsync(user);
        await db.Language.AddRangeAsync(languages);
        await db.SaveChangesAsync();
        var existingLanguages = new List<UserLanguage>
        {
            new(DateTime.UtcNow, user.Id, languages[2].Id),
            new(DateTime.UtcNow, user.Id, languages[0].Id)
        };
        await db.UserLanguage.AddRangeAsync(existingLanguages);
        await db.SaveChangesAsync();
        var request = new SaveUserLanguagesRequest(
            user.Id, 
            [
                new SaveUserLanguageItem(languages[2].Id, DateTime.UtcNow),
                new SaveUserLanguageItem(languages[1].Id, DateTime.UtcNow)
            ]);


        // Act
        await _userLanguageService.SaveUserLanguages(request);

        // Assert
        Assert.True(await db.UserLanguage.AnyAsync(ul => 
            ul.UserId == user.Id 
            && ul.LanguageId == languages[1].Id));

        Assert.False(await db.UserLanguage.AnyAsync(ul =>
            ul.UserId == user.Id
            && ul.LanguageId == languages[0].Id));
    }
}
