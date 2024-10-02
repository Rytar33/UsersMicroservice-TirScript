using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.News;

public record NewsListRequest(string? Search, int? TagId, PageRequest? Page);