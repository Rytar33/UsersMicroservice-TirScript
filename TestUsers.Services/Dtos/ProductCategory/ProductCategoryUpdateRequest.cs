namespace TestUsers.Services.Dtos.ProductCategory;

public record ProductCategoryUpdateRequest(int Id, string Name, int? ParentCategoryId);