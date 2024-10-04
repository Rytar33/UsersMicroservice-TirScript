using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Dtos;

namespace TestUsers.Services.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса сохранение фильтра пользователя
/// </summary>
public interface IUserSaveFilterService
{
    /// <summary>
    /// Получение списка сохранений пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<UserSaveFilterListItem>> GetList(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранение фильтра пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResponse> SaveFilter(UserSaveFilterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление фильтра пользователя
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default);
}
