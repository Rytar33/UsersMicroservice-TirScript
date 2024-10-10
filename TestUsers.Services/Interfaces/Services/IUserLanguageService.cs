using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.UserLanguages;

namespace TestUsers.Services.Interfaces.Services;

public interface IUserLanguageService
{
    /// <summary>
    /// Получить список изучаемых языков пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<UserLanguageItemResponse>> GetList(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить один язык к пользователю
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResponse> AddLanguageToUser(AddLanguageToUser request, Guid? sessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить список языков пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResponse> SaveUserLanguages(SaveUserLanguagesRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default);
}