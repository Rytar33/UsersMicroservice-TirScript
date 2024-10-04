namespace TestUsers.Services.Dtos.UserSaveFilters;

public record UserSaveFilterListItem(
    int Id,
    int UserId,
    string FilterName,
    List<int> CategoryParametersValuesIds,
    int? CategoryId,
    string? Search,
    decimal? FromAmount, 
    decimal? ToAmount,
    DateTime DateCreated);