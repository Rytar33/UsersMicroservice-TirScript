using TestUsers.Data.Enums;
using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Dtos.Users;

public record UsersListRequest(
    string? Search,
    EnumUserStatus? Status,
    PageRequest? Page);