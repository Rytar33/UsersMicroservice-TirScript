using Microsoft.EntityFrameworkCore;
using TestUsers.Data.Models;
using TestUsers.Data;
using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Exceptions;
using TestUsers.Services.Interfaces.Services;
using Xunit;
using Bogus;

namespace TestUsers.Services.Tests;

public class UserSaveFilterServiceTests
{
    private readonly DataContext _dbContext;
    private readonly IUserSaveFilterService _userSaveFilterService;
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Faker _faker;

    public UserSaveFilterServiceTests()
    {
        _faker = new Faker();
        // Настройка контекста базы данных для тестов
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(nameof(UserSaveFilterServiceTests))
            .Options;

        _dbContext = new DataContext(_dbContextOptions);
        _userSaveFilterService = new UserSaveFilterService(_dbContext);
    }

    /// <summary>
    /// Тест проверяет, что метод GetList корректно возвращает список сохранённых фильтров пользователя
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetList_ShouldReturnUserSaveFilters_WhenFiltersExist()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var filter = new UserSaveFilter(user.Id, "Filter1", DateTime.UtcNow, null, "search", 100, 500);
        await _dbContext.UserSaveFilter.AddAsync(filter);
        await _dbContext.SaveChangesAsync();

        var filterRelation = new UserSaveFilterRelation(1, filter.Id);
        await _dbContext.UserSaveFilterRelation.AddAsync(filterRelation);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userSaveFilterService.GetList(user.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal(filter.FilterName, result[0].FilterName);
        Assert.Contains(1, result[0].CategoryParametersValuesIds);
    }

    /// <summary>
    /// Тест проверяет, что метод GetList выбрасывает исключение NotFoundException, если запрашиваемый пользователь не существует.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetList_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        int nonexistentUserId = 999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userSaveFilterService.GetList(nonexistentUserId));
    }
    /// <summary>
    /// Тест проверяет, что метод SaveFilter корректно добавляет новый фильтр, если фильтр с таким именем не существует.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SaveFilter_ShouldAddNewFilter_WhenFilterDoesNotExist()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var productCategory = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await _dbContext.ProductCategory.AddAsync(productCategory);
        await _dbContext.SaveChangesAsync();

        var parameter = new ProductCategoryParameter(_faker.Commerce.ProductName(), productCategory.Id);
        await _dbContext.ProductCategoryParameter.AddAsync(parameter);
        await _dbContext.SaveChangesAsync();

        var values = new List<ProductCategoryParameterValue> 
        {
            new("Value1", parameter.Id), new("Value2", parameter.Id)
        };
        await _dbContext.ProductCategoryParameterValue.AddRangeAsync(values);
        await _dbContext.SaveChangesAsync();

        var request = new UserSaveFilterRequest(user.Id, "NewFilter", [ 1, 2 ], null, "search", 100, 500);

        // Act
        await _userSaveFilterService.SaveFilter(request);

        // Assert
        var savedFilter = await _dbContext.UserSaveFilter.FirstOrDefaultAsync(usf => usf.UserId == user.Id && usf.FilterName == request.SaveFilterName);
        Assert.NotNull(savedFilter);
        Assert.Equal(request.SaveFilterName, savedFilter.FilterName);
        Assert.Equal(request.Search, savedFilter.Search);
    }

    /// <summary>
    /// Тест проверяет, что метод SaveFilter корректно обновляет существующий фильтр.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SaveFilter_ShouldUpdateExistingFilter_WhenFilterExists()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var existingFilter = new UserSaveFilter(user.Id, "ExistingFilter", DateTime.UtcNow, null, "search", 100, 500);
        await _dbContext.UserSaveFilter.AddAsync(existingFilter);
        await _dbContext.SaveChangesAsync();

        var request = new UserSaveFilterRequest(user.Id, "ExistingFilter", [ 3 ], 20, "updatedSearch", 200, 600);

        // Act
        await _userSaveFilterService.SaveFilter(request);

        // Assert
        var updatedFilter = await _dbContext.UserSaveFilter.FirstOrDefaultAsync(usf => usf.Id == existingFilter.Id);
        Assert.NotNull(updatedFilter);
        Assert.Equal(request.CategoryId, updatedFilter.CategoryId);
        Assert.Equal(request.Search, updatedFilter.Search);
    }

    /// <summary>
    /// Тест проверяет, что метод Delete корректно удаляет фильтр, если он существует.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Delete_ShouldRemoveFilter_WhenFilterExists()
    {
        // Arrange
        var user = FakeDataService.GetGenerationUser();
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var filter = new UserSaveFilter(user.Id, "FilterToDelete", DateTime.UtcNow, null, "search", 100, 500);
        await _dbContext.UserSaveFilter.AddAsync(filter);
        await _dbContext.SaveChangesAsync();

        // Act
        await _userSaveFilterService.Delete(filter.Id);

        // Assert
        var deletedFilter = await _dbContext.UserSaveFilter.FindAsync(filter.Id);
        Assert.Null(deletedFilter);
    }

    /// <summary>
    /// Тест проверяет, что метод Delete выбрасывает исключение NotFoundException, если фильтр не существует.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Delete_ShouldThrowNotFoundException_WhenFilterDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userSaveFilterService.Delete(999));
    }
}
