using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.ProductCategory;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class ProductCategoryWsController(IProductCategoryService _productCategoryService) : BaseWsController
{
    public async Task<List<ProductCategoryListItem>> GetListByParent(ProductCategoryGetListByParentRequest request)
    {
        return await _productCategoryService.GetListByParent(request);
    }

    public async Task<List<ProductCategoryTreeItem>> GetTreeByParent(GetTreeByParentRequest request)
    {
        return await _productCategoryService.GetTreeByParent(request.ParentCategoryId);
    }

    public async Task<List<ProductCategoryTreeItem>> GetTree()
    {
        return await _productCategoryService.GetTree();
    }

    public async Task<BaseResponse> Create(ProductCategoryCreateRequest request)
    {
        return await _productCategoryService.Create(request);
    }

    public async Task<BaseResponse> Update(ProductCategoryUpdateRequest request)
    {
        return await _productCategoryService.Update(request);
    }

    public async Task<BaseResponse> Delete(ProductCategoryDeleteRequest request)
    {
        return await _productCategoryService.Delete(request.Id);
    }
}