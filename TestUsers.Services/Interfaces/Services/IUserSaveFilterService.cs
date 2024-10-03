using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Dtos;

namespace TestUsers.Services.Interfaces.Services;

public interface IUserSaveFilterService
{
    Task<List<UserSaveFilterListItem<TJsonFilter>>> GetList<TJsonFilter>(int userId, CancellationToken cancellationToken = default) where TJsonFilter : class;

    Task<BaseResponse> SaveFilter<TJsonFilter>(UserSaveFilterRequest<TJsonFilter> request, CancellationToken cancellationToken = default) where TJsonFilter : class;

    Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default);
}
