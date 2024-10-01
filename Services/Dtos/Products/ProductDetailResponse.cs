using Services.Dtos.ProductCategoryParameters;

namespace Services.Dtos.Products;

public record ProductDetailResponse(
    int Id,
    string Name,
    DateTime DateCreated,
    decimal Amount,
    int CategoryId,
    int CategoryName,
    string Description,
    List<ProductCategoryParameterValueListItem> CategoryParametersValues);