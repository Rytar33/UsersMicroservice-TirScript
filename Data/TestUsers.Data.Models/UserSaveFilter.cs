namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Сохранённые фильтры для пользователей"
/// </summary>
public class UserSaveFilter : BaseEntity
{
    public UserSaveFilter(
        int userId,
        string filterName,
        string filterValueJson,
        DateTime dateCreated) 
    {
        UserId = userId;
        FilterName = filterName;
        FilterValueJson = filterValueJson;
        DateCreated = dateCreated;
    }

    /// <summary>
    /// Пользователь
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Название фильтра
    /// </summary>
    public string FilterName { get; set; }

    /// <summary>
    /// Значение фильтра в JSON строке
    /// </summary>
    public string FilterValueJson { get; set; }

    /// <summary>
    /// Дата создание фильтра
    /// </summary>
    public DateTime DateCreated { get; set; }
}
