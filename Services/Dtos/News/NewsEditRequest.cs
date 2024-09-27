namespace Services.Dtos.News;

public record NewsEditRequest(
    int Id,
    string Title,
    string Description,
    int AuthorId,
    string Tags);