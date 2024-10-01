using AutoMapper;
using Models;
using Services.Dtos.ProductCategoryParameters;
using Services.Dtos.Products;

namespace Services.MappingProfiles; 

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductSaveRequest, Product>()
            .ForMember(p => p.Id, p => p.MapFrom(psr => psr.Id ?? 0))
            .ForMember(p => p.ProductCategory.Id, p => p.MapFrom(psr => psr.CategoryId))
            .ForMember(p => p.ProductCategory.Name, p => p.MapFrom(psr => psr.CategoryName));

        CreateMap<Product, ProductDetailResponse>()
            .ForMember(p => p.CategoryId, p => p.MapFrom(product => product.ProductCategory.Id))
            .ForMember(p => p.CategoryName, p => p.MapFrom(product => product.ProductCategory.Name))
            .ForMember(p => p.CategoryParametersValues, p => 
                p.MapFrom(product => 
                    product.ProductCategoryParameterValueProduct.Select(pcpvp => 
                        new ProductCategoryParameterValueListItem(
                            pcpvp.ProductCategoryParameterValue.ProductCategoryParameterId,
                            pcpvp.ProductCategoryParameterValue.ProductCategoryParameter.Name,
                            pcpvp.ProductCategoryParameterValue.Value))));
    }
}