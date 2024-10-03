using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.Products;

public record ProductListRequest(
    List<int> CategoryParametersValuesIds,
    int? CategoryId = null,
    string? Search = null,
    decimal? FromAmount = null,
    decimal? ToAmount = null,
    PageRequest? Page = null);