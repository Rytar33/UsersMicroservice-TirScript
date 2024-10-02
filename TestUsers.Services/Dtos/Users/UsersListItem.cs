using TestUsers.Data.Enums;

namespace TestUsers.Services.Dtos.Users;

public record UsersListItem(
    int Id,
    string Email,
    string FullName,
    DateTime DateRegister,
    EnumUserStatus Status);