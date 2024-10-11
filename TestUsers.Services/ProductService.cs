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

public class ProductService(DataContext _db, IUserSaveFilterService _userSaveFilterService) : IProductService
{
    public async Task<ProductListResponse> GetList(ProductListRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        await new ProductListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var productsForConditions = _db.Product.AsNoTracking();

        if (request.CategoryParametersValuesIds != null)
            productsForConditions = productsForConditions.Where(p =>
                p.ProductCategoryParameterValueProduct.Any(p =>
                    request.CategoryParametersValuesIds.Contains(p.ProductCategoryParameterValueId)));

        if (!string.IsNullOrWhiteSpace(request.Search))
            productsForConditions = productsForConditions
                .Where(x => x.Name.Contains(request.Search) 
                    || x.Description.Contains(request.Search)
                    || x.ProductCategory.Name.Contains(request.Search));

        if (request.FromAmount.HasValue)
            productsForConditions = productsForConditions.Where(x => x.Amount >= request.FromAmount);

        if (request.ToAmount.HasValue)
            productsForConditions = productsForConditions.Where(x => x.Amount <= request.ToAmount);

        if (request.CategoryId.HasValue)
            productsForConditions = productsForConditions.Where(x => x.CategoryId == request.CategoryId);

        var countProducts = productsForConditions.Count();

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
            await _userSaveFilterService.SaveFilter(
                new UserSaveFilterRequest(
                    request.UserId.Value,
                    request.FilterName,
                    request.CategoryParametersValuesIds,
                    request.CategoryId,
                    request.Search,
                    request.FromAmount,
                    request.ToAmount),
                sessionId,
                cancellationToken);

        return new ProductListResponse(productsItems, new PageResponse(countProducts, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<ProductDetailResponse> GetDetail(int id, CancellationToken cancellationToken = default)
    {
        var product = await _db.Product.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));
        var productCategory = await _db.ProductCategory.AsNoTracking().FirstOrDefaultAsync(pc => pc.Id == product.CategoryId, cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategory)));
        var productValueChoise = await _db.ProductCategoryParameterValueProduct
            .Where(pcpvp => pcpvp.ProductId == id)
            .Select(pcpvp =>
                new ProductCategoryParameterValueListItem(
                    pcpvp.ProductCategoryParameterValue.ProductCategoryParameterId,
                    pcpvp.ProductCategoryParameterValue.ProductCategoryParameter.Name,
                    pcpvp.ProductCategoryParameterValue.Value)
            ).ToListAsync(cancellationToken);
        return new ProductDetailResponse(
            product.Id,
            product.Name,
            product.DateCreated,
            product.Amount,
            product.CategoryId,
            productCategory.Name,
            product.Description,
            productValueChoise);
    }

    public async Task<BaseResponse> Save(ProductSaveRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductSaveRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        if (request.Id == null)
        {
            if (await _db.Product.AnyAsync(p => p.Name == request.Name, cancellationToken))
                throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(Product.Name), nameof(Product)));
            var product = new Product(request.Name, request.Description, request.DateCreated, request.Amount, request.CategoryId);
            await _db.Product.AddAsync(product, cancellationToken);
            await _db.ProductCategoryParameterValueProduct.AddRangeAsync(
                request.CategoryParametersValuesIds
                .Distinct()
                .Select(cpvi => new ProductCategoryParameterValueProduct(cpvi, product.Id)),cancellationToken);
        }
        else
        {
            var product = await _db.Product.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));

            if (await _db.Product.AnyAsync(p => p.Name == request.Name && request.Id != p.Id, cancellationToken))
                throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(Product.Name), nameof(Product)));

            product.Name = request.Name;
            product.Description = request.Description;
            product.DateCreated = request.DateCreated;
            product.Amount = request.Amount;
            product.CategoryId = request.CategoryId;

            var productCategoryParameterValuesOnDelet = await _db.ProductCategoryParameterValueProduct
                .Where(pcpvp => 
                    pcpvp.ProductId == product.Id 
                    && !request.CategoryParametersValuesIds.Contains(pcpvp.ProductCategoryParameterValueId))
                .ToListAsync(cancellationToken);

            if (productCategoryParameterValuesOnDelet.Count > 0)
                _db.ProductCategoryParameterValueProduct.RemoveRange(productCategoryParameterValuesOnDelet);

            var productCategoryParameterValuesOnAdd = request.CategoryParametersValuesIds
                .Distinct()
                .Where(cpvi => !_db.ProductCategoryParameterValueProduct
                    .Any(pcpvp => pcpvp.ProductId == product.Id
                        && pcpvp.ProductCategoryParameterValueId == cpvi))
                .Select(cpvi => new ProductCategoryParameterValueProduct(cpvi, product.Id))
                .ToList();

            if (productCategoryParameterValuesOnAdd.Count > 0)
                await _db.ProductCategoryParameterValueProduct.AddRangeAsync(productCategoryParameterValuesOnAdd, cancellationToken);
        }
        
        await _db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        if (!_db.Database.IsInMemory())
        {
            var rowsRemoved = await _db.Product.Where(p => p.Id == id).ExecuteDeleteAsync(cancellationToken);
            if (rowsRemoved == 0)
                throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));
        }
        else
        {
            var product = await _db.Product.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Product)));
            _db.Product.Remove(product);
            await _db.SaveChangesAsync(cancellationToken);
        }
        return new BaseResponse();
    }
}