namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Выбранные значения для товара"
/// </summary>
public class ProductCategoryParameterValueProduct : BaseEntity
{
    public ProductCategoryParameterValueProduct(
        int productCategoryParameterValueId,
        int productId)
    {
        ProductCategoryParameterValueId = productCategoryParameterValueId;
        ProductId = productId;
    }

    /// <summary>
    /// Идентификатор значения
    /// </summary>
    public int ProductCategoryParameterValueId { get; set; }

    /// <summary>
    /// Значение параметра категории товара
    /// </summary>
    public ProductCategoryParameterValue ProductCategoryParameterValue { get; set; }

    /// <summary>
    /// Идентификатор товара
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Товар
    /// </summary>
    public Product Product { get; set; }
}