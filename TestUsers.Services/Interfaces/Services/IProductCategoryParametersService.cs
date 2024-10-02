using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.ProductCategoryParameters;

namespace TestUsers.Services.Interfaces.Services;

public interface IProductCategoryParametersService
{
    Task<List<ProductCategoryParameterListItem>> GetList(ProductCategoryParametersListRequest request, CancellationToken cancellationToken = default); //получить список параметров категории

    Task<ProductCategoryParameterDetailResponse> GetDetail(int id, CancellationToken cancellationToken = default); //получить список параметров категории

    Task<ProductCategoryParameterValuesListResponse> GetParameterValues(ProductCategoryParameterValuesListRequest request, CancellationToken cancellationToken = default);  //получить список значений параметра категории

    Task<BaseResponse> Create(ProductCategoryParameterCreateRequest request, CancellationToken cancellationToken = default);

    Task<BaseResponse> Update(ProductCategoryParameterUpdateRequest request, CancellationToken cancellationToken = default);

    Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default);
}