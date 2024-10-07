using Xunit;
using TestUsers.Data;
using TestUsers.Services.Exceptions;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Services.Dtos.ProductCategory;
using Microsoft.EntityFrameworkCore;
using Bogus;
using TestUsers.Data.Models;

namespace TestUsers.Services.Tests;

public class ProductCategoryServiceTests
{
    private readonly DataContext _dataContext;
    private readonly IProductCategoryService _productCategoryService;
    private readonly Faker _faker;

    public ProductCategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: nameof(ProductCategoryServiceTests))
            .Options;

        _dataContext = new DataContext(options);
        _productCategoryService = new ProductCategoryService(_dataContext);
        _faker = new Faker();
    }

    /// <summary>
    /// Тест для метода GetListByParent, проверяющий возврат правильного списка категорий.
    /// </summary>
    [Fact]
    public async Task GetListByParent_ShouldReturnProductCategories_WhenConditionsMatch()
    {
        // Arrange
        var parentCategory = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await _dataContext.ProductCategory.AddAsync(parentCategory);
        await _dataContext.SaveChangesAsync();

        var category = new ProductCategory(_faker.Commerce.Categories(1)[0], parentCategory.Id);
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();

        var request = new ProductCategoryGetListByParentRequest(parentCategory.Id, category.Name);

        // Act
        var result = await _productCategoryService.GetListByParent(request);

        // Assert
        Assert.Single(result);
        Assert.Equal(category.Name, result[0].Name);
    }

    /// <summary>
    /// Тест для метода GetTree, проверяющий построение дерева категорий.
    /// </summary>
    [Fact]
    public async Task GetTree_ShouldBuildCategoryTree()
    {
        // Arrange
        var parentCategory = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await _dataContext.ProductCategory.AddAsync(parentCategory);
        await _dataContext.SaveChangesAsync();

        var childCategory = new ProductCategory(_faker.Commerce.Categories(1)[0], parentCategory.Id);
        await _dataContext.ProductCategory.AddAsync(childCategory);
        await _dataContext.SaveChangesAsync();

        // Act
        var result = await _productCategoryService.GetTree();

        // Assert
        Assert.Contains(parentCategory.Name, result.Select(r => r.Name));
        Assert.True(result
            .Select(r => r.ChildCategories.Select(cc => cc.Name))
            .Where(r => r.Any(cc => cc == childCategory.Name))
            .Any());
    }

    /// <summary>
    /// Тест для метода Create, проверяющий успешное создание новой категории.
    /// </summary>
    [Fact]
    public async Task Create_ShouldAddNewCategory_WhenValidRequest()
    {
        // Arrange
        var request = new ProductCategoryCreateRequest(_faker.Commerce.Categories(1)[0], null);

        // Act
        await _productCategoryService.Create(request);

        // Assert
        var createdCategory = await _dataContext.ProductCategory.FirstOrDefaultAsync(pc => pc.Name == request.Name);
        Assert.NotNull(createdCategory);
        Assert.Equal(request.Name, createdCategory.Name);
    }

    /// <summary>
    /// Тест для метода Update, проверяющий обновление категории.
    /// </summary>
    [Fact]
    public async Task Update_ShouldModifyCategory_WhenValidRequest()
    {
        // Arrange
        var category = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();

        var request = new ProductCategoryUpdateRequest(category.Id, _faker.Commerce.Categories(1)[0], null);

        // Act
        await _productCategoryService.Update(request);

        // Assert
        var updatedCategory = await _dataContext.ProductCategory.FindAsync(category.Id);
        Assert.NotNull(updatedCategory);
        Assert.Equal(request.Name, updatedCategory.Name);
    }

    /// <summary>
    /// Тест для метода Delete, проверяющий удаление категории.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldRemoveCategory_WhenCategoryExists()
    {
        // Arrange
        var category = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();

        // Act
        await _productCategoryService.Delete(category.Id);

        // Assert
        var deletedCategory = await _dataContext.ProductCategory.FindAsync(category.Id);
        Assert.Null(deletedCategory);
    }

    /// <summary>
    /// Тест для метода Delete, проверяющий выброс исключения, если категория не найдена.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldThrowNotFoundException_WhenCategoryNotFound()
    {
        // Arrange
        var nonExistentCategoryId = _faker.Random.Int(1000, 2000);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _productCategoryService.Delete(nonExistentCategoryId));
    }
}
