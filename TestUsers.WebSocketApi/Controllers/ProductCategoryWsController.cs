using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.ProductCategory;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class ProductCategoryWsController(IProductCategoryService productCategoryService) : BaseWsController
{
    public async Task<List<ProductCategoryListItem>> GetListByParent(ProductCategoryGetListByParentRequest request)
    {
        return await productCategoryService.GetListByParent(request);
    }

    public async Task<List<ProductCategoryTreeItem>> GetTree()
    {
        return await productCategoryService.GetTree();
    }

    public async Task<BaseResponse> Create(ProductCategoryCreateRequest request)
    {
        return await productCategoryService.Create(request);
    }

    public async Task<BaseResponse> Update(ProductCategoryUpdateRequest request)
    {
        return await productCategoryService.Update(request);
    }

    public async Task<BaseResponse> Delete(ProductCategoryDeleteRequest request)
    {
        return await productCategoryService.Delete(request.Id);
    }
}