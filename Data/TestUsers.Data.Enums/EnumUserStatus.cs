﻿using System.Text.Json.Serialization;

namespace TestUsers.Data.Enums;

/// <summary>
/// Статус пользователя
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<EnumUserStatus>))]
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