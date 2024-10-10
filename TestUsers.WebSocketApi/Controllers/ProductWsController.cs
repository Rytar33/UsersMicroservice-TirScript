using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Products;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class ProductWsController(IProductService productService) : BaseWsController
{
    public async Task<ProductListResponse> GetList(ProductListRequest request)
    {
        return await productService.GetList(request, Socket.SessionId);
    }

    public async Task<ProductDetailResponse> GetDetail(ProductDetailRequest request)
    {
        return await productService.GetDetail(request.IdProduct);
    }

    public async Task<BaseResponse> Save(ProductSaveRequest request)
    {
        return await productService.Save(request);
    }

    public async Task<BaseResponse> Delete(ProductDeleteRequest request)
    {
        return await productService.Delete(request.ProductId);
    }
}