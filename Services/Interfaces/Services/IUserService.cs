using Services.Dtos;
using Services.Dtos.Users;
using Services.Dtos.Users.Recoveries;

namespace Services.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса пользователя
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Получение постранично пользователей ассинхронно
    /// </summary>
    /// <param name="request">Фильтрация, номер страницы и размер страницы</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Возвращает список пользователей с номером страницы, размер страницы и количество полученных пользователей</returns>
    Task<UsersListResponse> GetList(UsersListRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение детальной информации об пользователе ассинхронно
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task<UserDetailResponse> GetDetail(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание пользователя ассинхронно
    /// </summary>
    /// <param name="request">Данные для создание пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task<BaseResponse> Create(UserCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование пользователя ассинхронно
    /// </summary>
    /// <param name="request">Данные запрашиваемые на изменение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task<BaseResponse> Edit(UserEditRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление пользователя ассинхронно
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task<BaseResponse> Delete(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Восстановление пароля аккаунта
    /// </summary>
    /// <param name="request">Запрос с почтой и если не укажет токен пользователь, то сгенерирует ему новый, иначе проверит</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task<BaseResponse> RecoveryStart(RecoveryStartRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Восстановление с изменением пароля аккаунта и сбросом токена
    /// </summary>
    /// <param name="request">Запрос с почтой, токеном, новым паролем и повторением пароля</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task<BaseResponse> RecoveryEnd(RecoveryEndRequest request, CancellationToken cancellationToken = default);
}