namespace TestUsers.Services.Dtos.ProductCategoryParameters;

public record ProductCategoryParameterDetailResponse(
    int Id,
    string Name,
    int ProductCategoryId,
    string ProductCategoryName,
    List<ProductCategoryParameterValueListItem> ParameterValues);