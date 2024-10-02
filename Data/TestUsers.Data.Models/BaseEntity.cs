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

    public override bool Equals(object? obj)
        => obj is BaseEntity entity 
           && Equals(entity);

    public override int GetHashCode()
        => Id.GetHashCode();
}