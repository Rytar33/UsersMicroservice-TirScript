namespace TestUsers.Data.Models;

/// <summary>
/// Абстрактный класс сущности
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}