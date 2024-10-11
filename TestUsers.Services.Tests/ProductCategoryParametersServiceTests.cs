using Xunit;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Exceptions;
using TestUsers.Services.Interfaces.Services;
using Bogus;
using TestUsers.Data.Models;

namespace TestUsers.Services.Tests;

public class ProductCategoryParametersServiceTests
{
    private readonly DataContext _dbContext;
    private readonly IProductCategoryParametersService _service;
    private readonly Faker _faker;

    public ProductCategoryParametersServiceTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: nameof(ProductCategoryParametersServiceTests))
            .Options;

        _dbContext = new DataContext(options);
        _service = new ProductCategoryParametersService(_dbContext);
        _faker = new Faker();
    }

    /// <summary>
    /// Тест для метода GetList, проверяющий возврат списка параметров.
    /// </summary>
    [Fact]
    public async Task GetList_ShouldReturnListOfParameters_WhenValidRequest()
    {
        // Arrange
        var productCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Categories(1)[0]));
        await _dbContext.ProductCategory.AddAsync(productCategory);
        await _dbContext.SaveChangesAsync();

        var parameter = new ProductCategoryParameter(FakeDataService.GetUniqueName(_faker.Commerce.ProductName()), productCategory.Id);
        await _dbContext.ProductCategoryParameter.AddAsync(parameter);
        await _dbContext.SaveChangesAsync();

        var request = new ProductCategoryParametersListRequest(parameter.Name, productCategory.Id);

        // Act
        var result = await _service.GetList(request);

        // Assert
        Assert.Contains(result, r => r.Name == parameter.Name);
    }

    /// <summary>
    /// Тест для метода GetDetail, проверяющий возврат детали параметра.
    /// </summary>
    [Fact]
    public async Task GetDetail_ShouldReturnParameterDetail_WhenValidId()
    {
        // Arrange
        var productCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Categories(1)[0]));
        var parameter = new ProductCategoryParameter(FakeDataService.GetUniqueName(_faker.Commerce.ProductName()), productCategory.Id);
        await _dbContext.ProductCategory.AddAsync(productCategory);
        await _dbContext.ProductCategoryParameter.AddAsync(parameter);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetDetail(parameter.Id);

        // Assert
        Assert.Equal(parameter.Name, result.Name);
        Assert.Equal(parameter.Id, result.Id);
    }

    /// <summary>
    /// Тест для метода Create, проверяющий успешное создание параметра.
    /// </summary>
    [Fact]
    public async Task Create_ShouldAddNewParameter_WhenValidRequest()
    {
        // Arrange
        var productCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Categories(1)[0]));
        await _dbContext.ProductCategory.AddAsync(productCategory);
        await _dbContext.SaveChangesAsync();

        var request = new ProductCategoryParameterCreateRequest(FakeDataService.GetUniqueName(_faker.Commerce.ProductName()), productCategory.Id, [FakeDataService.GetUniqueName("Value1"), FakeDataService.GetUniqueName("Value2") ]);

        // Act
        var result = await _service.Create(request);

        // Assert
        var createdParameter = await _dbContext.ProductCategoryParameter.FirstOrDefaultAsync(p => p.Name == request.Name);
        Assert.NotNull(createdParameter);
        Assert.Equal(request.Name, createdParameter.Name);
    }

    /// <summary>
    /// Тест для метода Update, проверяющий успешное обновление параметра.
    /// </summary>
    [Fact]
    public async Task Update_ShouldModifyParameter_WhenValidRequest()
    {
        // Arrange
        var productCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Categories(1)[0]));
        var parameter = new ProductCategoryParameter(FakeDataService.GetUniqueName(_faker.Commerce.ProductName()), productCategory.Id);
        await _dbContext.ProductCategory.AddAsync(productCategory);
        await _dbContext.ProductCategoryParameter.AddAsync(parameter);
        await _dbContext.SaveChangesAsync();

        var request = new ProductCategoryParameterUpdateRequest(parameter.Id, FakeDataService.GetUniqueName(_faker.Commerce.ProductName()), productCategory.Id, [FakeDataService.GetUniqueName("Value1"), FakeDataService.GetUniqueName("Value2") ]);

        // Act
        await _service.Update(request);

        // Assert
        var updatedParameter = await _dbContext.ProductCategoryParameter.FirstOrDefaultAsync(pcp => pcp.Id == parameter.Id);
        Assert.NotNull(updatedParameter);
        Assert.Equal(request.Name, updatedParameter.Name);
    }

    /// <summary>
    /// Тест для метода Delete, проверяющий удаление параметра.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldRemoveParameter_WhenValidId()
    {
        // Arrange
        var productCategory = new ProductCategory(FakeDataService.GetUniqueName(_faker.Commerce.Categories(1)[0]));
        var parameter = new ProductCategoryParameter(FakeDataService.GetUniqueName(_faker.Commerce.ProductName()), productCategory.Id);
        await _dbContext.ProductCategory.AddAsync(productCategory);
        await _dbContext.ProductCategoryParameter.AddAsync(parameter);
        await _dbContext.SaveChangesAsync();

        // Act
        await _service.Delete(parameter.Id);

        // Assert
        var deletedParameter = await _dbContext.ProductCategoryParameter.FirstOrDefaultAsync(pcp => pcp.Id == parameter.Id);
        Assert.Null(deletedParameter);
    }

    /// <summary>
    /// Тест для метода Delete, проверяющий выброс исключения, если параметр не найден.
    /// </summary>
    [Fact]
    public async Task Delete_ShouldThrowNotFoundException_WhenParameterDoesNotExist()
    {
        // Arrange
        var nonExistentParameterId = _faker.Random.Int(1000, 2000);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.Delete(nonExistentParameterId));
    }
}
