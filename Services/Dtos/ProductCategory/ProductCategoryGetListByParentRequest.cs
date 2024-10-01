namespace Services.Dtos.ProductCategory;

public record ProductCategoryGetListByParentRequest(int? ParentCategoryId, string Search);