namespace TestUsers.Services.Dtos.ProductCategory;

public record ProductCategoryCreateRequest(string Name, int? ParentCategoryId);