using Bogus;
using TestUsers.Data;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos.Products;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Services.Exceptions;
using Xunit;
using TestUsers.Services.Dtos.Pages;

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
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();

        var products = new List<Product>
        {
            new(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), DateTime.Now, _faker.Random.Decimal(10, 500), category.Id),
            new(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), DateTime.Now, _faker.Random.Decimal(10, 500), category.Id)
        };
        await _dataContext.Product.AddRangeAsync(products);
        await _dataContext.SaveChangesAsync();

        var user = FakeDataService.GetGenerationUser();
        await _dataContext.User.AddAsync(user);
        await _dataContext.SaveChangesAsync();

        var request = new ProductListRequest(
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
        Assert.Contains(products[0].Name, result.Items.Select(i => i.Name));

        // Проверка, что фильтр был сохранен в базу
        var savedFilter = await _dataContext.UserSaveFilter.FirstOrDefaultAsync(usf => usf.UserId == user.Id && usf.FilterName == "TestFilter");

        Assert.NotNull(savedFilter);
        Assert.Equal(request.FilterName, savedFilter.FilterName);
        Assert.Equal(request.Search, savedFilter.Search);
    }


    /// <summary>
    /// Тест для метода GetDetail, проверяющий возвращение детальной информации по продукту.
    /// </summary>
    [Fact]
    public async Task GetDetail_ShouldReturnProductDetail_WhenProductExists()
    {
        // Arrange
        var category = new ProductCategory(_faker.Commerce.Categories(1)[0]);
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();

        var product = new Product(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), DateTime.Now, _faker.Random.Decimal(10, 500), category.Id);
        await _dataContext.Product.AddAsync(product);
        await _dataContext.SaveChangesAsync();

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
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();
        var parameter = new ProductCategoryParameter(_faker.Commerce.ProductMaterial(), category.Id);
        await _dataContext.ProductCategoryParameter.AddAsync(parameter);
        await _dataContext.SaveChangesAsync();
        var values = new List<ProductCategoryParameterValue> 
        { 
            new(_faker.Commerce.Color(), parameter.Id),
            new(_faker.Commerce.Color(), parameter.Id),
        };
        await _dataContext.ProductCategoryParameterValue.AddRangeAsync(values);
        await _dataContext.SaveChangesAsync();

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
        var product = await _dataContext.Product.FirstOrDefaultAsync(p => p.Name == request.Name);
        Assert.NotNull(product);
        Assert.Equal(request.Name, product.Name);
        Assert.True(await _dataContext.ProductCategoryParameterValueProduct.AnyAsync(pcpvp => 
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
        await _dataContext.ProductCategory.AddAsync(category);
        await _dataContext.SaveChangesAsync();

        var product = new Product(_faker.Commerce.ProductName(), _faker.Commerce.ProductDescription(), DateTime.Now, _faker.Random.Decimal(10, 500), category.Id);
        await _dataContext.Product.AddAsync(product);
        await _dataContext.SaveChangesAsync();

        // Act
        await _productService.Delete(product.Id);

        // Assert
        Assert.False(await _dataContext.Product.AnyAsync(p => p.Id == product.Id));
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
