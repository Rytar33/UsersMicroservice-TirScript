using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.ProductCategoryParameters;

public record ProductCategoryParameterValuesListRequest(
    string Search,
    int ProductCategoryParameterId,
    PageRequest? Page);