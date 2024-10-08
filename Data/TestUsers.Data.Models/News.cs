namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Новость"
/// </summary>
public class News : BaseEntity
{
    public News(
        string title,
        string description,
        DateTime dateCreated,
        int authorId)
    {
        Title = title;
        Description = description;
        DateCreated = dateCreated;
        AuthorId = authorId;
    }
    
    /// <summary>
    /// Главное название
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Автор новости
    /// </summary>
    public User Author { get; set; }

    /// <summary>
    /// Идентификатор автора
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// Теги новости
    /// </summary>
    public List<NewsTagRelation> Tags { get; set; } = [];
}