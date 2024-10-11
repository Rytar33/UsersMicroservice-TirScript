using TestUsers.Data.Models;
using TestUsers.Services.Extensions;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Pages;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Dtos.Validators.ProductCategoryParameters;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Services.Exceptions;
using TestUsers.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace TestUsers.Services;

public class ProductCategoryParametersService(DataContext _db) : IProductCategoryParametersService
{
    public async Task<List<ProductCategoryParameterListItem>> GetList(ProductCategoryParametersListRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryParametersListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var parametersCategory = _db.ProductCategoryParameter.Where(pcp => pcp.ProductCategoryId == request.ProductCategoryId);
        if (!string.IsNullOrWhiteSpace(request.Search))
            parametersCategory = parametersCategory.Where(pcp => pcp.Name.Contains(request.Search));
        return await parametersCategory
            .Select(pc => 
                new ProductCategoryParameterListItem(
                    pc.Id, 
                    pc.Name,
                    pc.ProductCategoryId, 
                    pc.ProductCategory.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductCategoryParameterDetailResponse> GetDetail(int id, CancellationToken cancellationToken = default)
    {
        var parameter = await _db.ProductCategoryParameter.AsNoTracking().FirstOrDefaultAsync(pcp => pcp.Id == id, cancellationToken) 
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));

        var values = await _db.ProductCategoryParameterValue
            .Where(v => v.ProductCategoryParameterId == parameter.Id)
            .Select(v => new ProductCategoryParameterValueListItem(parameter.Id, parameter.Name, v.Value))
            .ToListAsync(cancellationToken);

        var category = await _db.ProductCategory.AsNoTracking().FirstOrDefaultAsync(pc => pc.Id == id, cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategory)));

        return new ProductCategoryParameterDetailResponse(
            parameter.Id,
            parameter.Name,
            parameter.ProductCategoryId,
            category.Name,
            values);
    }

    public async Task<ProductCategoryParameterValuesListResponse> GetParameterValues(ProductCategoryParameterValuesListRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryParameterValuesListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var valuesForConditions = _db.ProductCategoryParameterValue.Where(pcpv =>
            pcpv.ProductCategoryParameterId == request.ProductCategoryParameterId
            && pcpv.Value.Contains(request.Search)
            || pcpv.ProductCategoryParameter.Name.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.Search))
            valuesForConditions = valuesForConditions.Where(pcpv => 
                pcpv.Value.Contains(request.Search)
                || pcpv.ProductCategoryParameter.Name.Contains(request.Search));

        var countValuesParameter = valuesForConditions.Count();

        if (request.Page != null)
            valuesForConditions = valuesForConditions.GetPage(request.Page);

        var productsItems = valuesForConditions.Select(v => new ProductCategoryParameterValueListItem(v.ProductCategoryParameterId, v.ProductCategoryParameter.Name, v.Value)).ToList();

        return new ProductCategoryParameterValuesListResponse(productsItems, new PageResponse(countValuesParameter, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<BaseResponse> Create(ProductCategoryParameterCreateRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryParameterCreateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        if (await _db.ProductCategoryParameter.AnyAsync(pcp => pcp.Name == request.Name, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(ProductCategoryParameter.Name), nameof(ProductCategoryParameter)));
        var parameter = new ProductCategoryParameter(request.Name, request.ProductCategoryId);
        await _db.ProductCategoryParameter.AddAsync(parameter, cancellationToken);

        var values = request.Values
            .Distinct()
            .Select(v => new ProductCategoryParameterValue(v, parameter.Id))
            .ToList();
        if (values.Count > 0)
            await _db.ProductCategoryParameterValue.AddRangeAsync(values, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Update(ProductCategoryParameterUpdateRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryParameterUpdateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var parameter = await _db.ProductCategoryParameter.FirstOrDefaultAsync(pcp => pcp.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));

        if (await _db.ProductCategoryParameter.AnyAsync(pcp => pcp.Name == request.Name && pcp.Id != request.Id, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(ProductCategoryParameter.Name), nameof(ProductCategoryParameter)));

        parameter.Name = request.Name;
        parameter.ProductCategoryId = request.ProductCategoryId;

        var valuesOnDelet = await _db.ProductCategoryParameterValue
            .Where(v => 
                v.ProductCategoryParameterId == parameter.Id 
                && !request.Values.Any(value => value == v.Value))
            .ToListAsync(cancellationToken);
        if (valuesOnDelet.Count > 0)
            _db.ProductCategoryParameterValue.RemoveRange(valuesOnDelet);

        var valuesOnCreate = await _db.ProductCategoryParameterValue
            .Where(v =>
                v.ProductCategoryParameterId == parameter.Id
                && request.Values.Any(value => value == v.Value))
            .ToListAsync(cancellationToken);
        if (valuesOnCreate.Count > 0)
            await _db.ProductCategoryParameterValue.AddRangeAsync(valuesOnCreate, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        if (!_db.Database.IsInMemory())
        {
            var rowsRemoved = await _db.ProductCategoryParameter.Where(pcp => pcp.Id == id).ExecuteDeleteAsync(cancellationToken);
            if (rowsRemoved == 0)
                throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));
        }
        else
        {
            var productCategoryParameter = await _db.ProductCategoryParameter.FirstOrDefaultAsync(pcp => pcp.Id == id, cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));
            _db.ProductCategoryParameter.Remove(productCategoryParameter);
            await _db.SaveChangesAsync(cancellationToken);
        }
        return new BaseResponse();
    }
}