namespace TestUsers.Data.Enums;

/// <summary>
/// Статус пользователя
/// </summary>
public enum EnumUserStatus
{
    /// <summary>
    /// Не подтвержден
    /// </summary>
    NotConfirmed = 0,

    /// <summary>
    /// Активный
    /// </summary>
    Active = 1,

    /// <summary>
    /// Заблокированный
    /// </summary>
    Locked = 2
}