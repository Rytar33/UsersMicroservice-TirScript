namespace TestUsers.Data.Models;

/// <summary>
/// Сущность "Отношение многие ко многим между сохранением фильтра пользователя и значениями параметров категории товаров"
/// </summary>
public class UserSaveFilterRelation : BaseEntity
{
    public UserSaveFilterRelation(int productCategoryParameterValueId, int userSaveFilterId) 
    {
        ProductCategoryParameterValueId = productCategoryParameterValueId;
        UserSaveFilterId = userSaveFilterId;
    }

    /// <summary>
    /// Идентификатор значения
    /// </summary>
    public int ProductCategoryParameterValueId { get; set; }

    /// <summary>
    /// Значение параметра категории под товары
    /// </summary>
    public ProductCategoryParameterValue ProductCategoryParameterValue { get; set; }

    /// <summary>
    /// Идентификатор фильтра пользователя
    /// </summary>
    public int UserSaveFilterId { get; set; }

    /// <summary>
    /// Фильтр пользователя
    /// </summary>
    public UserSaveFilter UserSaveFilter { get; set; }
}