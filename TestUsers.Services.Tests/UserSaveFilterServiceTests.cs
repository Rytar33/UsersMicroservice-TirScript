using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestUsers.Data.Models;
using TestUsers.Data;
using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Exceptions;
using TestUsers.Services.Interfaces.Services;
using Xunit;

namespace TestUsers.Services.Tests;

public class UserSaveFilterServiceTests
{
    private readonly DataContext _dbContext;
    private readonly IUserSaveFilterService _userSaveFilterService;
    private readonly DbContextOptions<DataContext> _dbContextOptions;

    public UserSaveFilterServiceTests()
    {
        // Настройка контекста базы данных для тестов
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(nameof(UserSaveFilterServiceTests))
            .Options;

        _dbContext = new DataContext(_dbContextOptions);
        _userSaveFilterService = new UserSaveFilterService(_dbContext);
    }

    [Fact]
    public async Task GetList_ShouldReturnUserSaveFilters_WhenFiltersExist()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var filter = new UserSaveFilter(user.Id, "Filter1", "{\"key\":\"value\"}", DateTime.UtcNow);
        await _dbContext.UserSaveFilter.AddAsync(filter);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userSaveFilterService.GetList<Dictionary<string, string>>(user.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal(filter.FilterName, result[0].FilterName);
        Assert.Equal(JsonConvert.DeserializeObject<Dictionary<string, string>>(filter.FilterValueJson), result[0].FilterValues);
    }

    [Fact]
    public async Task GetList_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        int nonexistentUserId = 999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userSaveFilterService.GetList<Dictionary<string, string>>(nonexistentUserId));
    }

    [Fact]
    public async Task SaveFilter_ShouldAddNewFilter_WhenFilterDoesNotExist()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var request = new UserSaveFilterRequest<Dictionary<string, string>>(user.Id, "NewFilter", new Dictionary<string, string> { { "key", "value" } });

        // Act
        await _userSaveFilterService.SaveFilter(request);

        // Assert
        var savedFilter = await _dbContext.UserSaveFilter.FirstOrDefaultAsync(usf => usf.UserId == user.Id && usf.FilterName == request.SaveFilterName);
        Assert.NotNull(savedFilter);
        Assert.Equal(request.SaveFilterName, savedFilter.FilterName);
        Assert.Equal(JsonConvert.SerializeObject(request.SaveFilterValue), savedFilter.FilterValueJson);
    }

    [Fact]
    public async Task SaveFilter_ShouldUpdateExistingFilter_WhenFilterExists()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var existingFilter = new UserSaveFilter(user.Id, "ExistingFilter", "{\"key\":\"value1\"}", DateTime.UtcNow);
        await _dbContext.UserSaveFilter.AddAsync(existingFilter);
        await _dbContext.SaveChangesAsync();

        var request = new UserSaveFilterRequest<Dictionary<string, string>>(user.Id, "ExistingFilter", new Dictionary<string, string> { { "key", "value2" } });

        // Act
        await _userSaveFilterService.SaveFilter(request);

        // Assert
        var updatedFilter = await _dbContext.UserSaveFilter.FirstOrDefaultAsync(usf => usf.Id == existingFilter.Id);
        Assert.NotNull(updatedFilter);
        Assert.Equal(JsonConvert.SerializeObject(request.SaveFilterValue), updatedFilter.FilterValueJson);
    }

    [Fact]
    public async Task Delete_ShouldRemoveFilter_WhenFilterExists()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var filter = new UserSaveFilter(user.Id, "FilterToDelete", "{\"key\":\"value\"}", DateTime.UtcNow);
        await _dbContext.UserSaveFilter.AddAsync(filter);
        await _dbContext.SaveChangesAsync();

        // Act
        await _userSaveFilterService.Delete(filter.Id);

        // Assert
        var deletedFilter = await _dbContext.UserSaveFilter.FindAsync(filter.Id);
        Assert.Null(deletedFilter);
    }

    [Fact]
    public async Task Delete_ShouldThrowNotFoundException_WhenFilterDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userSaveFilterService.Delete(999));
    }
}
