namespace Services.Dtos.Pages;

public record PageResponse(int Count, int Page = 1, int PageSize = 10);