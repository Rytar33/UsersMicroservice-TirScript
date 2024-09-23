using Enums;
using Services.Dtos.Pages;

namespace Services.Dtos.Users;

public record UsersListRequest(
    string? Search,
    EnumUserStatus? Status,
    PageRequest Page);