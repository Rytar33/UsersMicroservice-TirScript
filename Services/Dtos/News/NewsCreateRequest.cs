namespace Services.Dtos.News;

public record NewsCreateRequest(
    string Title,
    string Description,
    int AuthorId,
    string Tags);