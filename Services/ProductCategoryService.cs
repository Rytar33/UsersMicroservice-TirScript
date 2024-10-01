using AutoMapper;
using Models;
using Models.Exceptions;
using Models.Extensions;
using Models.Validations;
using Services.Dtos;
using Services.Dtos.ProductCategory;
using Services.Dtos.Validators.ProductCategories;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services;

public class ProductCategoryService(
    IMapper mapper,
    IProductCategoryRepository productCategoryRepository) : IProductCategoryService
{
    public async Task<List<ProductCategoryListItem>> GetListByParent(ProductCategoryGetListByParentRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductCategoryGetListByParentRequestValidator().ValidateWithErrors(request);
        var productCategories = await productCategoryRepository.GetListByExpression(pc => 
            pc.Name.Contains(request.Search) 
            && pc.ParentCategoryId == request.ParentCategoryId,
            cancellationToken);

        return productCategories.Select(mapper.Map<ProductCategory, ProductCategoryListItem>).ToList();
    }

    public async Task<List<ProductCategoryTreeItem>> GetTree(CancellationToken cancellationToken = default)
    {
        var productCategories = await productCategoryRepository
            .GetListByExpression(cancellationToken: cancellationToken);
        return productCategories.Select(mapper.Map<ProductCategory, ProductCategoryTreeItem>).ToList();
    }

    public async Task<BaseResponse> Create(ProductCategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductCategoryCreateRequestValidator().ValidateWithErrors(request);
        await productCategoryRepository.CreateAsync(mapper.Map<ProductCategoryCreateRequest, ProductCategory>(request), cancellationToken);
        await productCategoryRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Update(ProductCategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductCategoryUpdateRequestValidator().ValidateWithErrors(request);
        await productCategoryRepository.UpdateAsync(mapper.Map<ProductCategoryUpdateRequest, ProductCategory>(request), cancellationToken);
        await productCategoryRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        var productCategory = await productCategoryRepository.GetByExpression(pc => pc.Id == id, cancellationToken);
        if (productCategory == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategory)));
        productCategoryRepository.Delete(productCategory);
        await productCategoryRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }
}