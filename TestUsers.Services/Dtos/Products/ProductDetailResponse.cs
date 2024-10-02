using TestUsers.Services.Dtos.ProductCategoryParameters;

namespace TestUsers.Services.Dtos.Products;

public record ProductDetailResponse(
    int Id,
    string Name,
    DateTime DateCreated,
    decimal Amount,
    int CategoryId,
    string CategoryName,
    string Description,
    List<ProductCategoryParameterValueListItem> CategoryParametersValues);