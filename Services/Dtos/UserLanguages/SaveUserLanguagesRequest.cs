namespace Services.Dtos.UserLanguages;

public record SaveUserLanguagesRequest(int UserId, List<SaveUserLanguageItem> Languages);