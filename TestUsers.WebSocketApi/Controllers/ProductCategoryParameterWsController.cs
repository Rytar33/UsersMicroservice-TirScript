using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class ProductCategoryParameterWsController(IProductCategoryParametersService _productCategoryParametersService) : BaseWsController
{
    public async Task<List<ProductCategoryParameterListItem>> GetList(ProductCategoryParametersListRequest request)
    {
        return await _productCategoryParametersService.GetList(request);
    }

    public async Task<ProductCategoryParameterValuesListResponse> GetParameterValues(ProductCategoryParameterValuesListRequest request)
    {
        return await _productCategoryParametersService.GetParameterValues(request);
    }

    public async Task<ProductCategoryParameterDetailResponse> GetDetail(ProductCategoryParameterDetailRequest request)
    {
        return await _productCategoryParametersService.GetDetail(request.Id);
    }

    public async Task<BaseResponse> Create(ProductCategoryParameterCreateRequest request)
    {
        return await _productCategoryParametersService.Create(request);
    }

    public async Task<BaseResponse> Update(ProductCategoryParameterUpdateRequest request)
    {
        return await _productCategoryParametersService.Update(request);
    }

    public async Task<BaseResponse> Delete(ProductCategoryParameterDeleteRequest request)
    {
        return await _productCategoryParametersService.Delete(request.Id);
    }
}