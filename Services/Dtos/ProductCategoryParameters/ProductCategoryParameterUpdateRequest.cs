namespace Services.Dtos.ProductCategoryParameters;

public record ProductCategoryParameterUpdateRequest(int Id, string Name, int ProductCategoryId, List<string> Values);