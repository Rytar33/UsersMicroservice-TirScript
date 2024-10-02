namespace TestUsers.Data.Models;

/// <summary>
/// Связь многие ко многим между сущностям "Теги" и "Новости"
/// </summary>
public class NewsTagRelation : BaseEntity
{
    public NewsTagRelation(int newsId, int newsTagId)
    {
        NewsId = newsId;
        NewsTagId = newsTagId;
    }
    
    /// <summary>
    /// Идентификатор новости
    /// </summary>
    public int NewsId { get; set; }

    /// <summary>
    /// Новость
    /// </summary>
    public News News { get; set; }

    /// <summary>
    /// Идентификатор тега
    /// </summary>
    public int NewsTagId { get; set; }

    /// <summary>
    /// Тег
    /// </summary>
    public NewsTag NewsTag { get; set; }
}