using Services.Dtos.Pages;

namespace Services.Dtos.ProductCategoryParameters;

public record ProductCategoryParameterValuesListRequest(string Search, int ProductCategoryParameterId, PageRequest? Page);