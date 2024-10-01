using AutoMapper;
using Models;
using Models.Exceptions;
using Models.Extensions;
using Models.Validations;
using Services.Dtos;
using Services.Dtos.Pages;
using Services.Dtos.Products;
using Services.Dtos.Validators.Products;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services;

public class ProductService(
    IMapper mapper,
    IProductRepository productRepository,
    IProductCategoryParameterValueProductRepository productCategoryParameterValueProductRepository) : IProductService
{
    public async Task<ProductListResponse> GetList(ProductListRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductListRequestValidator().ValidateWithErrors(request);
        var products = await productRepository.GetListByExpression(cancellationToken: cancellationToken, includes: [ p => p.ProductCategory, p => p.ProductCategoryParameterValueProduct ]);
        var productsForConditions = products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            productsForConditions = productsForConditions
                .Where(x => 
                    x.Name.Contains(request.Search) 
                    || x.Description.Contains(request.Search)
                    || x.ProductCategory.Name.Contains(request.Search));

        if (request.FromAmount.HasValue)
            productsForConditions = productsForConditions.Where(x => x.Amount >= request.FromAmount);

        if (request.ToAmount.HasValue)
            productsForConditions = productsForConditions.Where(x => x.Amount <= request.ToAmount);

        if (request.CategoryId.HasValue)
            productsForConditions = productsForConditions.Where(x => x.CategoryId == request.CategoryId);

        productsForConditions = productsForConditions.Where(x 
            => x.ProductCategoryParameterValueProduct.Exists(p => request.CategoryParametersValuesIds.Contains(p.ProductCategoryParameterValueId)));

        var countProducts = productsForConditions.Count();

        if (request.Page != null)
            productsForConditions = productsForConditions
            .Skip((request.Page.Page - 1) * request.Page.PageSize)
                .Take(request.Page.PageSize);

        var productsItems = productsForConditions.Select(p => mapper.Map<Product, ProductListItem>(p)).ToList();

        return new ProductListResponse(productsItems, new PageResponse(countProducts, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<ProductDetailResponse> GetDetail(int id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByExpression(p => 
            p.Id == id,
            cancellationToken, 
            p => p.ProductCategory,
            p => p.ProductCategory.Parameters,
            p => p.ProductCategory.Parameters, 
            p => p.ProductCategory.Parameters.Select(pc => pc.Values));
        if (product == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));
        return mapper.Map<Product, ProductDetailResponse>(product);
    }

    public async Task<BaseResponse> Save(ProductSaveRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductSaveRequestValidator().ValidateWithErrors(request);

        var product = mapper.Map<ProductSaveRequest, Product>(request);

        try
        {
            await productRepository.StartTransaction(cancellationToken);
            if (product.Id == 0)
                await productRepository.CreateAsync(product, cancellationToken);
            else
                await productRepository.UpdateAsync(product, cancellationToken);
            await productRepository.SaveChangesAsync(cancellationToken);

            foreach (var productCategoryParameterValueProduct in product.ProductCategoryParameterValueProduct.Where(
                         productCategoryParameterValueProduct =>
                             !request.CategoryParametersValuesIds.Contains(productCategoryParameterValueProduct.Id)))
            {
                productCategoryParameterValueProductRepository.Delete(productCategoryParameterValueProduct);
                await productCategoryParameterValueProductRepository.SaveChangesAsync(cancellationToken);
            }

            foreach (var valueId in request.CategoryParametersValuesIds.Where(valuesId => 
                         !product.ProductCategoryParameterValueProduct.Any(pcpvp => 
                             pcpvp.ProductCategoryParameterValueId == valuesId)))
            {
                await productCategoryParameterValueProductRepository.CreateAsync(
                    new ProductCategoryParameterValueProduct(valueId, product.Id), cancellationToken);
                await productCategoryParameterValueProductRepository.SaveChangesAsync(cancellationToken);
            }

            await productRepository.CommitTransaction(cancellationToken);
            return new BaseResponse();
        }
        catch (Exception)
        {
            await productRepository.RollBackTransaction(cancellationToken);
            throw;
        }
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        var getProduct = await productRepository.GetByExpression(p => p.Id == id, cancellationToken, p => p.ProductCategoryParameterValueProduct);
        if (getProduct == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));
        productRepository.Delete(getProduct);
        await productRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }
}