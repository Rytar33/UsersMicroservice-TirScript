namespace Services.Dtos.Products;

public record ProductListItem(
    int Id,
    string Name,
    DateTime DateCreated,
    decimal Amount,
    int CategoryId,
    int CategoryName);