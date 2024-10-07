namespace TestUsers.Services.Dtos.UserSaveFilters;

public record UserSaveFilterRequest(
    int UserId, 
    string SaveFilterName,
    List<int>? CategoryParametersValuesIds = null,
    int? CategoryId = null,
    string? Search = null,
    decimal? FromAmount = null,
    decimal? ToAmount = null);