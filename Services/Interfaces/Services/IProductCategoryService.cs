using Services.Dtos;
using Services.Dtos.ProductCategory;

namespace Services.Interfaces.Services;

public interface IProductCategoryService
{
    Task<List<ProductCategoryListItem>> GetListByParent(ProductCategoryGetListByParentRequest request, CancellationToken cancellationToken = default);  //получить список категорий по родительской категории

    Task<List<ProductCategoryTreeItem>> GetTree(CancellationToken cancellationToken = default);  //получить дерево категорий

    Task<BaseResponse> Create(ProductCategoryCreateRequest request, CancellationToken cancellationToken = default);

    Task<BaseResponse> Update(ProductCategoryUpdateRequest request, CancellationToken cancellationToken = default);

    Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default);
}