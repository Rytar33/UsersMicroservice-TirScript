using AutoMapper;
using Models;
using Services.Dtos.ProductCategoryParameters;

namespace Services.MappingProfiles;

public class ProductCategoryParameterProfile : Profile
{
    public ProductCategoryParameterProfile()
    {
        CreateMap<ProductCategoryParameterCreateRequest, ProductCategoryParameter>();

        CreateMap<ProductCategoryParameterUpdateRequest, ProductCategoryParameter>();

        CreateMap<ProductCategory, List<ProductCategoryParameterListItem>>();

        CreateMap<ProductCategoryParameter, ProductCategoryParameterDetailResponse>();
    }
}