using Services.Dtos.Pages;

namespace Services.Dtos.News;

public record NewsListResponse(List<NewsListItem> Items, PageResponse Page);