namespace Services.Dtos.News;

public record NewsDetailResponse(
    int Id,
    string Title,
    string Description,
    DateTime DateCreated,
    List<NewsTagResponse> Tags);