namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Роль"
/// </summary>
public class Role : BaseEntity
{
    public Role(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Список пользователей использующую эту роль
    /// </summary>
    public List<UserRole> UserRoles { get; set; } = [];
}