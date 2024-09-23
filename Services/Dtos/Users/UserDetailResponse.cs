using Enums;

namespace Services.Dtos.Users;

public record UserDetailResponse(
    int Id,
    string Email,
    string FullName,
    DateTime DateRegister,
    EnumUserStatus Status,
    bool IsSuccess = true, 
    string? ErrorMessage = null) : BaseResponse(IsSuccess, ErrorMessage);