namespace Services.Dtos.Products;

public record ProductSaveRequest(
    int? Id,
    string Name,
    DateTime DateCreated,
    decimal Amount,
    int CategoryId,
    string CategoryName,
    string Description,
    List<int> CategoryParametersValuesIds);