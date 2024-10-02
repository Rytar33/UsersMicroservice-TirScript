using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.Products;

public record ProductListResponse(List<ProductListItem> Items, PageResponse Page);