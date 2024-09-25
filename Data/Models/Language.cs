namespace Models;

/// <summary>
/// Сущность "Язык"
/// </summary>
public class Language : BaseEntity
{
    public Language(string code, string name)
    {
        Code = code;
        Name = name;
    }

    /// <summary>
    /// Код языка ("ru", "en" и т.д.)
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Название языка
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Список пользователей, владеющих языком
    /// </summary>
    public List<UserLanguage> Users { get; set; } = [];
}