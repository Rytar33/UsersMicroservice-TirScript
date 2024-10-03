namespace TestUsers.Services.Dtos.UserSaveFilters;

public record UserSaveFilterRequest<TJsonFilter>(int UserId, string SaveFilterName, TJsonFilter SaveFilterValue) where TJsonFilter : class;