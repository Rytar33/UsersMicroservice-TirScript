namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Параметры категорий" (например вес, размер, цвет и т.д)
/// </summary>
public class ProductCategoryParameter : BaseEntity
{
    public ProductCategoryParameter(string name, int productCategoryId)
    {
        Name = name;
        ProductCategoryId = productCategoryId;
    }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Категория товара под параметр
    /// </summary>
    public ProductCategory ProductCategory { get; set; }

    /// <summary>
    /// Идентификатор категории
    /// </summary>
    public int ProductCategoryId { get; set; }

    /// <summary>
    /// Значении параметра
    /// </summary>
    public List<ProductCategoryParameterValue> Values { get; set; } = [];
}