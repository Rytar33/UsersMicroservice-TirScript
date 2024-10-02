using TestUsers.Services.Dtos.Pages;

namespace TestUsers.Services.Extensions;

public static class IQuerieableExtensions
{
    public static IQueryable<T> GetPage<T>(this IQueryable<T> collection, PageRequest pageRequest)
        => collection.Skip((pageRequest.Page - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);
}