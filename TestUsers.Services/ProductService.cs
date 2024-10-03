using TestUsers.Data.Models;
using TestUsers.Services.Extensions;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Pages;
using TestUsers.Services.Dtos.Products;
using TestUsers.Services.Dtos.Validators.Products;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Data;
using TestUsers.Services.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Dtos.UserSaveFilters;

namespace TestUsers.Services;

public class ProductService(DataContext db, IUserSaveFilterService userSaveFilterService) : IProductService
{
    public async Task<ProductListResponse> GetList(ProductListRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var productsForConditions = db.Product.Where(p => 
            p.ProductCategoryParameterValueProduct.Exists(p => 
                request.CategoryParametersValuesIds.Contains(p.ProductCategoryParameterValueId)));

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

        var countProducts = await productsForConditions.CountAsync(cancellationToken);

        if (request.Page != null)
            productsForConditions = productsForConditions.GetPage(request.Page);

        var productsItems = productsForConditions
            .Select(p => 
                new ProductListItem(
                    p.Id,
                    p.Name,
                    p.DateCreated,
                    p.Amount,
                    p.CategoryId,
                    p.ProductCategory.Name))
            .ToList();

        if (request.SaveFilter && !string.IsNullOrWhiteSpace(request.FilterName) && request.UserId.HasValue)
            await userSaveFilterService.SaveFilter(new UserSaveFilterRequest<ProductListRequest>(request.UserId.Value, request.FilterName, request), cancellationToken);
        return new ProductListResponse(productsItems, new PageResponse(countProducts, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<ProductDetailResponse> GetDetail(int id, CancellationToken cancellationToken = default)
    {
        var product = await db.Product.FindAsync([id], cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));
        return new ProductDetailResponse(
            product.Id,
            product.Name,
            product.DateCreated,
            product.Amount,
            product.CategoryId,
            product.ProductCategory.Name,
            product.Description,
            product.ProductCategoryParameterValueProduct.Select(pcpvp => 
                new ProductCategoryParameterValueListItem(
                    pcpvp.ProductCategoryParameterValue.ProductCategoryParameterId,
                    pcpvp.ProductCategoryParameterValue.ProductCategoryParameter.Name,
                    pcpvp.ProductCategoryParameterValue.Value)
            ).ToList());
    }

    public async Task<BaseResponse> Save(ProductSaveRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductSaveRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        if (request.Id == null)
        {
            if (await db.Product.AnyAsync(p => p.Name == request.Name, cancellationToken))
                throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(Product.Name), nameof(Product)));
            var product = new Product(request.Name, request.Description, request.DateCreated, request.Amount, request.CategoryId);
            await db.Product.AddAsync(product, cancellationToken);
            await db.ProductCategoryParameterValueProduct.AddRangeAsync(
                request.CategoryParametersValuesIds
                .Distinct()
                .Select(cpvi => new ProductCategoryParameterValueProduct(cpvi, product.Id)),cancellationToken);
        }
        else
        {
            var product = await db.Product.FindAsync([ request.Id ], cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));

            if (await db.Product.AnyAsync(p => p.Name == request.Name && request.Id != p.Id, cancellationToken))
                throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(Product.Name), nameof(Product)));

            product.Name = request.Name;
            product.Description = request.Description;
            product.DateCreated = request.DateCreated;
            product.Amount = request.Amount;
            product.CategoryId = request.CategoryId;

            var productCategoryParameterValuesOnDelet = await db.ProductCategoryParameterValueProduct
                .Where(pcpvp => 
                    pcpvp.ProductId == product.Id 
                    && !request.CategoryParametersValuesIds.Contains(pcpvp.ProductCategoryParameterValueId))
                .ToListAsync(cancellationToken);

            if (productCategoryParameterValuesOnDelet.Count > 0)
                db.ProductCategoryParameterValueProduct.RemoveRange(productCategoryParameterValuesOnDelet);

            var productCategoryParameterValuesOnAdd = request.CategoryParametersValuesIds
                .Distinct()
                .Where(cpvi => !db.ProductCategoryParameterValueProduct
                    .Any(pcpvp => pcpvp.ProductId == product.Id
                        && pcpvp.ProductCategoryParameterValueId == cpvi))
                .Select(cpvi => new ProductCategoryParameterValueProduct(cpvi, product.Id))
                .ToList();

            if (productCategoryParameterValuesOnAdd.Count > 0)
                await db.ProductCategoryParameterValueProduct.AddRangeAsync(productCategoryParameterValuesOnAdd, cancellationToken);
        }
        
        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        var rowsRemoved = await db.Product.Where(p => p.Id == id).ExecuteDeleteAsync(cancellationToken);
        if (rowsRemoved == 0)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));
        return new BaseResponse();
    }
}