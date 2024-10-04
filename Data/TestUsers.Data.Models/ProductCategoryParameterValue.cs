namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "значение параметра категории"
/// (например вес 5кг, 10кг, размер X, Xl, XXl, цвет Green, Red и т.д)
/// </summary>
public class ProductCategoryParameterValue : BaseEntity
{
    public ProductCategoryParameterValue(string value, int productCategoryParameterId)
    {
        Value = value;
        ProductCategoryParameterId = productCategoryParameterId;
    }

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Параметр для значения
    /// </summary>
    public ProductCategoryParameter ProductCategoryParameter { get; set; }

    /// <summary>
    /// Идентификатор параметра
    /// </summary>
    public int ProductCategoryParameterId { get; set; }

    /// <summary>
    /// Выбранные значения для товара
    /// </summary>
    public List<ProductCategoryParameterValueProduct> ProductCategoryParameterValueProduct { get; set; } = [];

    /// <summary>
    /// Пользователи сохранившие идентификатор значения параметра
    /// </summary>
    public List<UserSaveFilterRelation> UserSaveFilter { get; set; } = [];
}