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
        var languages = new List<Language> { new(FakeDataService.GetUniqueName("en"), FakeDataService.GetUniqueName("English")), new(FakeDataService.GetUniqueName("ro"), FakeDataService.GetUniqueName("Romanian")) };
        await _dbContext.User.AddAsync(user);
        await _dbContext.Language.AddRangeAsync(languages);
        await _dbContext.SaveChangesAsync();
        var userLanguages = new List<UserLanguage>
        {
            new(DateTime.UtcNow, user.Id, languages[0].Id),
            new(DateTime.UtcNow, user.Id, languages[1].Id)
        };
        await _dbContext.UserLanguage.AddRangeAsync(userLanguages);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userLanguageService.GetList(user.Id);

        // Assert
        Assert.Equal(result.Count, userLanguages.Count);
        Assert.Contains(result, r => r.Name == languages[0].Name);
        Assert.Contains(result, r => r.Code == languages[1].Code);
    }

    /// <summary>
    /// Проверяет, добавляется ли язык пользователю
    /// </summary>
    [Fact]
    public async Task AddLanguageToUser_ShouldAddLanguage_WhenValidRequestIsPassed()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        var language = new Language(FakeDataService.GetUniqueName("en"), FakeDataService.GetUniqueName("English"));
        await _dbContext.User.AddAsync(user);
        await _dbContext.Language.AddAsync(language);
        await _dbContext.SaveChangesAsync();
        var request = new AddLanguageToUser(user.Id, language.Id, DateTime.UtcNow);

        // Act
        await _userLanguageService.AddLanguageToUser(request);

        // Assert
        Assert.True(await _dbContext.UserLanguage.AnyAsync(ul => ul.LanguageId == language.Id && ul.UserId == user.Id));
    }

    /// <summary>
    /// Проверяет сохранение списка языков пользователя
    /// </summary>
    [Fact]
    public async Task SaveUserLanguages_ShouldSaveNewLanguagesAndDeleteOld_OnValidRequest()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        var languages = new List<Language> { new(FakeDataService.GetUniqueName("en"), FakeDataService.GetUniqueName("English")), new(FakeDataService.GetUniqueName("ro"), FakeDataService.GetUniqueName("Romanian")), new(FakeDataService.GetUniqueName("ru"), FakeDataService.GetUniqueName("Russian")) };
        await _dbContext.User.AddAsync(user);
        await _dbContext.Language.AddRangeAsync(languages);
        await _dbContext.SaveChangesAsync();
        var existingLanguages = new List<UserLanguage>
        {
            new(DateTime.UtcNow, user.Id, languages[2].Id),
            new(DateTime.UtcNow, user.Id, languages[0].Id)
        };
        await _dbContext.UserLanguage.AddRangeAsync(existingLanguages);
        await _dbContext.SaveChangesAsync();
        var request = new SaveUserLanguagesRequest(
            user.Id, 
            [
                new SaveUserLanguageItem(languages[2].Id, DateTime.UtcNow),
                new SaveUserLanguageItem(languages[1].Id, DateTime.UtcNow)
            ]);


        // Act
        await _userLanguageService.SaveUserLanguages(request);

        // Assert
        Assert.True(await _dbContext.UserLanguage.AnyAsync(ul => 
            ul.UserId == user.Id 
            && ul.LanguageId == languages[1].Id));

        Assert.False(await _dbContext.UserLanguage.AnyAsync(ul =>
            ul.UserId == user.Id
            && ul.LanguageId == languages[0].Id));
    }
}
