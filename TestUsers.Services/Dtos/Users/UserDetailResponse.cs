using TestUsers.Data.Enums;

namespace TestUsers.Services.Dtos.Users;

public record UserDetailResponse(
    int Id,
    string Email,
    string FullName,
    DateTime DateRegister,
    EnumUserStatus Status);