using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.Users;

public record UsersListResponse(
    List<UsersListItem> Items,
    PageResponse Page);
