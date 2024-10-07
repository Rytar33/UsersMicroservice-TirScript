using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.Products;

public record ProductListRequest(
    List<int>? CategoryParametersValuesIds = null,
    int? CategoryId = null,
    string? Search = null,
    decimal? FromAmount = null,
    decimal? ToAmount = null,
    bool SaveFilter = false,
    string? FilterName = null,
    int? UserId = null,
    PageRequest? Page = null);