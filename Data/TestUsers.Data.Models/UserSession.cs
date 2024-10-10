namespace TestUsers.Data.Models;

/// <summary>
/// Сессия пользователя в веб сокете
/// </summary>
public class UserSession : BaseEntity
{
    public UserSession(int userId, Guid sessionId)
    {
        UserId = userId;
        SessionId = sessionId;
    }

    /// <summary>
    /// Идентификатор сессии
    /// </summary>
    public Guid SessionId { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Сессия пользователя
    /// </summary>
    public User User { get; set; }
}
