namespace Services.Dtos.Users;

public record UserCreateRequest(string Email, string FullName, string Password);