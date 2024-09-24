using Enums;

namespace Models;

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
        DateRegister = DateTime.Now;
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
}