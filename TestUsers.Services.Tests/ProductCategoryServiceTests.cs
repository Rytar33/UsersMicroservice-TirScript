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
        var parentCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()));
        await _dataContext.ProductCategory.AddAsync(parentCategory);
        await _dataContext.SaveChangesAsync();

        var category = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()), parentCategory.Id);
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
        var parentCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()));
        await _dataContext.ProductCategory.AddAsync(parentCategory);
        await _dataContext.SaveChangesAsync();

        var childCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()), parentCategory.Id);
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
    /// Тест для метода GetTreeByParent, проверяющий построение дерева категорий по родителю.
    /// </summary>
    [Fact]
    public async Task GetTreeByParent_ShouldBuildCategoryTreeByParent_WhenParentIdIsProvided()
    {
        // Arrange
        var parentCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()));
        await _dataContext.ProductCategory.AddAsync(parentCategory);
        await _dataContext.SaveChangesAsync();

        var childCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()), parentCategory.Id);
        await _dataContext.ProductCategory.AddAsync(childCategory);
        await _dataContext.SaveChangesAsync();

        var grandChildCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()), childCategory.Id);
        await _dataContext.ProductCategory.AddAsync(grandChildCategory);
        await _dataContext.SaveChangesAsync();

        // Act
        var result = await _productCategoryService.GetTreeByParent(parentCategory.Id);

        // Assert
        Assert.Single(result); // Должен быть только один родитель
        var parentNode = result.First();
        Assert.Equal(parentCategory.Name, parentNode.Name);
        Assert.Single(parentNode.ChildCategories); // Должен быть один дочерний элемент

        var childNode = parentNode.ChildCategories.First();
        Assert.Equal(childCategory.Name, childNode.Name);
        Assert.Single(childNode.ChildCategories); // У дочернего элемента также должен быть свой дочерний элемент

        var grandChildNode = childNode.ChildCategories.First();
        Assert.Equal(grandChildCategory.Name, grandChildNode.Name);
        Assert.Empty(grandChildNode.ChildCategories); // У внука не должно быть дочерних элементов
    }

    /// <summary>
    /// Тест для метода GetTreeByParent, проверяющий возврат всего дерева категорий, если родитель не указан.
    /// </summary>
    [Fact]
    public async Task GetTreeByParent_ShouldReturnFullTree_WhenParentIdIsNull()
    {
        // Arrange
        var rootCategory1 = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()));
        var rootCategory2 = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()));

        await _dataContext.ProductCategory.AddRangeAsync(rootCategory1, rootCategory2);
        await _dataContext.SaveChangesAsync();

        var childCategory1 = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()), rootCategory1.Id);
        var childCategory2 = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()), rootCategory2.Id);

        await _dataContext.ProductCategory.AddRangeAsync(childCategory1, childCategory2);
        await _dataContext.SaveChangesAsync();

        // Act
        var result = await _productCategoryService.GetTreeByParent(null);

        // Assert
        Assert.Contains(result, r => r.Name == rootCategory1.Name);
        Assert.Contains(result, r => r.Name == rootCategory2.Name);
    }


    /// <summary>
    /// Тест для метода Create, проверяющий успешное создание новой категории.
    /// </summary>
    [Fact]
    public async Task Create_ShouldAddNewCategory_WhenValidRequest()
    {
        // Arrange
        var request = new ProductCategoryCreateRequest(_faker.Commerce.Product(), null);

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
        var category = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()));
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();

        var request = new ProductCategoryUpdateRequest(category.Id, _faker.Commerce.Product(), null);

        // Act
        await _productCategoryService.Update(request);

        // Assert
        var updatedCategory = await _dataContext.ProductCategory.FirstOrDefaultAsync(pc => pc.Id == category.Id);
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
        var category = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Product()));
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();

        // Act
        await _productCategoryService.Delete(category.Id);

        // Assert
        var deletedCategory = await _dataContext.ProductCategory.FirstOrDefaultAsync(pc => pc.Id == category.Id);
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
