using Services.Dtos;
using Services.Dtos.UserLanguages;

namespace Services.Interfaces.Services;

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
    Task<BaseResponse> AddLanguageToUser(AddLanguageToUser request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить список языков пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResponse> SaveUserLanguages(SaveUserLanguagesRequest request, CancellationToken cancellationToken = default);
}