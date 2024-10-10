using TestUsers.Data.Enums;

namespace TestUsers.Data.Models;

/// <summary>
/// Класс сущности "Пользователь"
/// </summary>
public class User : BaseEntity
{
    public User(
        string email,
        string fullName,
        string passwordHash)
    {
        Email = email;
        FullName = fullName;
        PasswordHash = passwordHash;
        DateRegister = DateTime.UtcNow;
    }

    /// <summary>
    /// Электронная почта
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// ФИО
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Хэш пароля
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime DateRegister { get; }

    /// <summary>
    /// Статус пользователя
    /// </summary>
    public EnumUserStatus Status { get; set; }

    /// <summary>
    /// Токен для восстановления почты
    /// </summary>
    public string? RecoveryToken { get; set; }

    /// <summary>
    /// Список контактов пользователя
    /// </summary>
    public List<UserContact> Contacts { get; set; } = [];

    /// <summary>
    /// Список языков, которыми владеет пользователь
    /// </summary>
    public List<UserLanguage> UserLanguages { get; set; } = [];

    /// <summary>
    /// Список сохранённых фильтров
    /// </summary>
    public List<UserSaveFilter> SaveFilters { get; set; } = [];

    /// <summary>
    /// Список новостей, в которых пользователь является автором
    /// </summary>
    public List<News> NewsCreated { get; set; } = [];

    /// <summary>
    /// Список сессий пользователя
    /// </summary>
    public List<UserSession> Sessions { get; set; } = [];

    /// <summary>
    /// Роли пользователя
    /// </summary>
    public List<UserRole> UserRoles { get; set; } = [];
}