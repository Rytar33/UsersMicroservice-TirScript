using Services.Dtos.Pages;

namespace Services.Dtos.Products;

public record ProductListRequest(
    int? CategoryId,
    string? Search,
    decimal? FromAmount,
    decimal? ToAmount,
    List<int> CategoryParametersValuesIds,
    PageRequest? Page);