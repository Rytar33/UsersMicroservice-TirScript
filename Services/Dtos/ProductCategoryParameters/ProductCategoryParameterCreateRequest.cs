namespace Services.Dtos.ProductCategoryParameters;

public record ProductCategoryParameterCreateRequest(string Name, int ProductCategoryId, List<string> Values);