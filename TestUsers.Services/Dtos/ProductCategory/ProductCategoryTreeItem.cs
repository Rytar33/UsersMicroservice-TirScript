namespace TestUsers.Services.Dtos.ProductCategory;

public record ProductCategoryTreeItem(
    int Id,
    string Name,
    int? ParentCategoryId,
    List<ProductCategoryTreeItem> ChildCategories);