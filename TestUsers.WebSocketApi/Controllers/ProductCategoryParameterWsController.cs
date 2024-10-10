using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class ProductCategoryParameterWsController(IProductCategoryParametersService productCategoryParametersService) : BaseWsController
{
    public async Task<List<ProductCategoryParameterListItem>> GetList(ProductCategoryParametersListRequest request)
    {
        return await productCategoryParametersService.GetList(request);
    }

    public async Task<ProductCategoryParameterValuesListResponse> GetParameterValues(ProductCategoryParameterValuesListRequest request)
    {
        return await productCategoryParametersService.GetParameterValues(request);
    }

    public async Task<ProductCategoryParameterDetailResponse> GetDetail(ProductCategoryParameterDetailRequest request)
    {
        return await productCategoryParametersService.GetDetail(request.Id);
    }

    public async Task<BaseResponse> Create(ProductCategoryParameterCreateRequest request)
    {
        return await productCategoryParametersService.Create(request);
    }

    public async Task<BaseResponse> Update(ProductCategoryParameterUpdateRequest request)
    {
        return await productCategoryParametersService.Update(request);
    }

    public async Task<BaseResponse> Delete(ProductCategoryParameterDeleteRequest request)
    {
        return await productCategoryParametersService.Delete(request.Id);
    }
}