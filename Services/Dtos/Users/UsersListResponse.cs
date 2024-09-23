using Services.Dtos.Pages;

namespace Services.Dtos.Users;

public record UsersListResponse(
    List<UsersListItem> Items,
    PageResponse Page,
    bool IsSuccess = true,
    string? ErrorMessage = null) : BaseResponse(IsSuccess, ErrorMessage);
