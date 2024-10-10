namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Связь многие ко многим между пользователями и ролями"
/// </summary>
public class UserRole : BaseEntity
{
    public UserRole(int userId, int roleId) 
    {
        UserId = userId;
        RoleId = roleId;
    }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Пользователь
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Идентификатор роли
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Роль
    /// </summary>
    public Role Role { get; set; }
}