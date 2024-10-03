using Bogus;
using TestUsers.Data;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.Products;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Services.Exceptions;
using Xunit;
using TestUsers.Services.Dtos.Pages;
using Newtonsoft.Json;

namespace TestUsers.Services.Tests;

public class ProductServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly DataContext _dataContext;
    private readonly Faker _faker;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _faker = new Faker("ru");
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: nameof(ProductServiceTests))
            .Options;
        _dataContext = new DataContext(_dbContextOptions);
        _productService = new ProductService(_dataContext, new UserSaveFilterService(_dataContext));
    }

    /// <summary>
    /// Тест для метода GetList, проверяющий возвращаемый список продуктов и сохранение фильтров.
    /// </summary>
    [Fact]
    public async Task GetList_ShouldReturnProductsBasedOnSearchConditions_AndSaveFilterWhenFlagIsTrue()
    {
        // Arrange
        var category = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await using var db = new DataContext(_dbContextOptions);
        await db.ProductCategory.AddAsync(category);
        await db.SaveChangesAsync();

        var products = new List<Product>
        {
            new(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), DateTime.Now, _faker.Random.Decimal(10, 500), category.Id),
            new(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), DateTime.Now, _faker.Random.Decimal(10, 500), category.Id)
        };
        await db.Product.AddRangeAsync(products);
        await db.SaveChangesAsync();

        var user = FakeDataService.GetGenerationUser();
        await db.User.AddAsync(user);
        await db.SaveChangesAsync();

        var request = new ProductListRequest(
            CategoryParametersValuesIds: new List<int>(),
            Search: products[0].Name,  // Поиск по названию первого продукта
            Page: new PageRequest(1, 10),
            SaveFilter: true,          // Флаг сохранения фильтра
            FilterName: "TestFilter",  // Имя фильтра
            UserId: user.Id            // Пользователь, для которого сохраняем фильтр
        );

        // Act
        var result = await _productService.GetList(request);

        // Assert
        // Проверка, что продукт был возвращен корректно
        Assert.Single(result.Items);
        Assert.Equal(products[0].Name, result.Items[0].Name);

        // Проверка, что фильтр был сохранен в базу
        var savedFilter = await db.UserSaveFilter.FirstOrDefaultAsync(usf => usf.UserId == user.Id && usf.FilterName == "TestFilter");

        Assert.NotNull(savedFilter);
        Assert.Equal(request.FilterName, savedFilter.FilterName);
        Assert.Equal(JsonConvert.SerializeObject(request), savedFilter.FilterValueJson);
    }


    /// <summary>
    /// Тест для метода GetDetail, проверяющий возвращение детальной информации по продукту.
    /// </summary>
    [Fact]
    public async Task GetDetail_ShouldReturnProductDetail_WhenProductExists()
    {
        // Arrange
        var category = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await using var db = new DataContext(_dbContextOptions);
        await db.ProductCategory.AddAsync(category);
        await db.SaveChangesAsync();

        var product = new Product(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), DateTime.Now, _faker.Random.Decimal(10, 500), category.Id);
        await db.Product.AddAsync(product);
        await db.SaveChangesAsync();

        // Act
        var result = await _productService.GetDetail(product.Id);

        // Assert
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
    }

    /// <summary>
    /// Тест для метода GetDetail, проверяющий выброс исключения, если продукт не найден.
    /// </summary>
    [Fact]
    public async Task GetDetail_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        // Arrange
        var productId = _faker.Random.Int(1, 1000);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetDetail(productId));
    }

    /// <summary>
    /// Тест для метода Save, проверяющий создание нового продукта.
    /// </summary>
    [Fact]
    public async Task Save_ShouldCreateNewProduct_WhenValidRequest()
    {
        // Arrange
        var category = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await using var db = new DataContext(_dbContextOptions);
        await db.ProductCategory.AddAsync(category);
        await db.SaveChangesAsync();
        var parameter = new ProductCategoryParameter(_faker.Commerce.ProductMaterial(), category.Id);
        await db.ProductCategoryParameter.AddAsync(parameter);
        await db.SaveChangesAsync();
        var values = new List<ProductCategoryParameterValue> 
        { 
            new(_faker.Commerce.Color(), parameter.Id),
            new(_faker.Commerce.Color(), parameter.Id),
        };
        await db.ProductCategoryParameterValue.AddRangeAsync(values);
        await db.SaveChangesAsync();

        var request = new ProductSaveRequest(
            null,
            _faker.Commerce.ProductName(),
            DateTime.UtcNow,
            _faker.Random.Decimal(10, 500),
            category.Id,
            category.Name,
            _faker.Commerce.ProductDescription(),
            values.Select(values => values.Id).ToList());

        // Act
        await _productService.Save(request);

        // Assert
        var product = await db.Product.FirstOrDefaultAsync(p => p.Name == request.Name);
        Assert.NotNull(product);
        Assert.Equal(request.Name, product.Name);
        Assert.True(await db.ProductCategoryParameterValueProduct.AnyAsync(pcpvp => 
            pcpvp.ProductId == product.Id 
            && request.CategoryParametersValuesIds.Contains(pcpvp.ProductCategoryParameterValueId)));
    }

    /// <summary>
    /// Тест для метода Delete, проверяющий успешное удаление продукта.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldRemoveProduct_WhenProductExists()
    {
        // Arrange
        var category = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await using var db = new DataContext(_dbContextOptions);
        await db.ProductCategory.AddAsync(category);
        await db.SaveChangesAsync();

        var product = new Product(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), DateTime.Now, _faker.Random.Decimal(10, 500), category.Id);
        await db.Product.AddAsync(product);
        await db.SaveChangesAsync();

        // Act
        await _productService.Delete(product.Id);

        // Assert
        Assert.False(await db.Product.AnyAsync(p => p.Id == product.Id));
    }

    /// <summary>
    /// Тест для метода Delete, проверяющий выброс исключения, если продукт не найден.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        // Arrange
        var productId = _faker.Random.Int(1, 1000);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _productService.Delete(productId));
    }
}
