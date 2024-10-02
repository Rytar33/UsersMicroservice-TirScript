namespace TestUsers.Services.Dtos.ProductCategoryParameters;

public record ProductCategoryParameterListItem(
    int Id,
    string Name,
    int ProductCategoryId,
    string ProductCategoryName);