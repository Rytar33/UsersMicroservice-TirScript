namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Язык пользователя"
/// </summary>
public class UserLanguage : BaseEntity
{
    public UserLanguage(DateTime dateLearn, int userId, int languageId)
    {
        DateLearn = dateLearn;
        UserId = userId;
        LanguageId = languageId;
    }

    /// <summary>
    /// Пользователь владеющий этим языком
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Когда был изучен язык
    /// </summary>
    public DateTime DateLearn { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Идентификатор языка
    /// </summary>
    public int LanguageId { get; set; }

    /// <summary>
    /// Сам язык
    /// </summary>
    public Language Language { get; set; }
}