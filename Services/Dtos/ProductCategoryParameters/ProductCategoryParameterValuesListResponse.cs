using Services.Dtos.Pages;

namespace Services.Dtos.ProductCategoryParameters;

public record ProductCategoryParameterValuesListResponse(List<ProductCategoryParameterValueListItem> Items, PageResponse Page);