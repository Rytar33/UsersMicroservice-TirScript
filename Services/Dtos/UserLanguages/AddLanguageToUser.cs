namespace Services.Dtos.UserLanguages;

public record AddLanguageToUser(int UserId, int LanguageId, DateTime DateLearn);