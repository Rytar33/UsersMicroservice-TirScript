namespace TestUsers.Services.Dtos.UserSaveFilters;

public record UserSaveFilterListItem<TJsonObject>(
    int Id,
    int UserId,
    string FilterName,
    TJsonObject FilterValues,
    DateTime DateCreated) where TJsonObject : class;