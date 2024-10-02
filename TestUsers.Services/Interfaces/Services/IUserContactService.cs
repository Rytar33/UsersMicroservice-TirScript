using TestUsers.Services.Dtos.UserContacts;

namespace TestUsers.Services.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса контактов пользователя
/// </summary>
public interface IUserContactService
{
    /// <summary>
    /// Получение контактов
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Возвращает список контактов пользователя в виде объектов UserContactItem</returns>
    Task<List<UserContactItem>> GetContacts(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет контакты для пользователя, 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveContacts(UserContactsSaveRequest request, CancellationToken cancellationToken = default);
}