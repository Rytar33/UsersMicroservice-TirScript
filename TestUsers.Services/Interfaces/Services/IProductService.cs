using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Products;

namespace TestUsers.Services.Interfaces.Services;

public interface IProductService
{
    Task<ProductListResponse> GetList(ProductListRequest request, CancellationToken cancellationToken = default);  //получить список 

    Task<ProductDetailResponse> GetDetail(int id, CancellationToken cancellationToken = default);  //получить детальную

    Task<BaseResponse> Save(ProductSaveRequest request, CancellationToken cancellationToken = default);

    Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default);
}