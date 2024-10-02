namespace TestUsers.Services.Dtos.News;

public record NewsListItem(
    int Id,
    string Title,
    DateTime DateCreated,
    int AuthorId);