namespace Services.Dtos.Users.Recoveries;

public record RecoveryEndRequest(
    string Email,
    string RecoveryTokenRequest,
    string NewPassword,
    string RepeatNewPassword);