using AutoMapper;
using Models;
using Services.Dtos.ProductCategory;

namespace Services.MappingProfiles;

public class ProductCategoryProfile : Profile
{
    public ProductCategoryProfile()
    {
        CreateMap<ProductCategoryCreateRequest, ProductCategory>();

        CreateMap<ProductCategoryUpdateRequest, ProductCategory>();

        CreateMap<ProductCategory, ProductCategoryListItem>();

        CreateMap<ProductCategory, ProductCategoryTreeItem>();
    }
}