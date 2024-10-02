using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.ProductCategoryParameters;

public record ProductCategoryParameterValuesListResponse(
    List<ProductCategoryParameterValueListItem> Items,
    PageResponse Page);