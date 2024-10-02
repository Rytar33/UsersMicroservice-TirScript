namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Контакты пользователя"
/// </summary>
public class UserContact : BaseEntity
{
    public UserContact(
        string name,
        string value,
        int userId)
    {
        Name = name;
        Value = value;
        UserId = userId;
    }

    /// <summary>
    /// Тип контакта
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Значение контакта
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Пользователь к которому принадлежит контакт
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }
}