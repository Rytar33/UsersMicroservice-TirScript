using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.News;

public record NewsListResponse(List<NewsListItem> Items, PageResponse Page);