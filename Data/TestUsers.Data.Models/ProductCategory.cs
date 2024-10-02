namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Категория товаров"
/// </summary>
public class ProductCategory : BaseEntity
{
    public ProductCategory(
        string name,
        int? parentCategoryId = null)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
    }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Товары под эту категорию
    /// </summary>
    public List<Product> Products { get; set; } = [];

    /// <summary>
    /// Дочерние категории
    /// </summary>
    public List<ProductCategory> ChildCategories { get; set; } = [];

    /// <summary>
    /// Идентификатор родительской категории
    /// </summary>
    public int? ParentCategoryId { get; set; }

    /// <summary>
    /// Родительская категория
    /// </summary>
    public ProductCategory ParentCategory { get; set; }

    /// <summary>
    /// Параметры категории
    /// </summary>
    public List<ProductCategoryParameter> Parameters { get; set; } = [];
}