namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Сохранённые фильтры для пользователей"
/// </summary>
public class UserSaveFilter : BaseEntity
{
    public UserSaveFilter(
        int userId,
        string filterName,
        DateTime dateCreated,
        int? categoryId = null,
        string? search = null,
        decimal? fromAmount = null,
        decimal? toAmount = null)
    {
        UserId = userId;
        FilterName = filterName;
        DateCreated = dateCreated;
        CategoryId = categoryId;
        Search = search;
        FromAmount = fromAmount;
        ToAmount = toAmount;
    }

    /// <summary>
    /// Пользователь
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Название фильтра
    /// </summary>
    public string FilterName { get; set; }

    /// <summary>
    /// Значения параметров категории под товары
    /// </summary>
    public List<UserSaveFilterRelation> CategoryParametersValues { get; set; } = [];

    /// <summary>
    /// Идентификатор категории под товары
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Категории под товары
    /// </summary>
    public ProductCategory ProductCategory { get; set; }

    /// <summary>
    /// Строка поиска
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// От цены за товар
    /// </summary>
    public decimal? FromAmount { get; set; }

    /// <summary>
    /// До цен за товар
    /// </summary>
    public decimal? ToAmount { get; set; }

    /// <summary>
    /// Дата создание фильтра
    /// </summary>
    public DateTime DateCreated { get; set; }
}
