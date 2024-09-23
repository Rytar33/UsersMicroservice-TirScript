using Enums;

namespace Services.Dtos.Users;

public record UsersListItem(
    int Id,
    string Email,
    string FullName,
    DateTime DateRegister,
    EnumUserStatus Status);