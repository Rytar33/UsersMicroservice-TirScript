using Services.Dtos.Pages;

namespace Services.Dtos.Products;

public class ProductListResponse(List<ProductListItem> Items, PageResponse Page);