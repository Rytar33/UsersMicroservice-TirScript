using Services.Dtos.Pages;

namespace Services.Dtos.News;

public record NewsListRequest(string? Search, int? TagId, PageRequest? Page);