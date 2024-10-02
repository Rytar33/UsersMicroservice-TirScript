namespace TestUsers.Data.Models;

/// <summary>
/// Теги для новостей
/// </summary>
public class NewsTag : BaseEntity
{
    public NewsTag(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Список новостей который имеет этот тег
    /// </summary>
    public List<NewsTagRelation> News { get; set; } = [];
}