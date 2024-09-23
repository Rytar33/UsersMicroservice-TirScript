namespace Services.Dtos;

public record BaseResponse(bool IsSuccess = true, string? ErrorMessage = null);