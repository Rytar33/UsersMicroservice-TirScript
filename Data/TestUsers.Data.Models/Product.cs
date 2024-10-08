namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Товар"
/// </summary>
public class Product : BaseEntity
{
    public Product(
        string name,
        string description,
        DateTime dateCreated,
        decimal amount,
        int categoryId)
    {
        Name = name;
        Description = description;
        DateCreated = dateCreated;
        Amount = amount;
        CategoryId = categoryId;
    }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Цена за товар
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Выбранные значение параметра категории товара
    /// </summary>
    public List<ProductCategoryParameterValueProduct> ProductCategoryParameterValueProduct { get; set; } = [];

    /// <summary>
    /// Идентификатор категории товара
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Категория товара
    /// </summary>
    public ProductCategory ProductCategory { get; set; }
}