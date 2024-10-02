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

public class ProductCategoryParametersService(DataContext db) : IProductCategoryParametersService
{
    public async Task<List<ProductCategoryParameterListItem>> GetList(ProductCategoryParametersListRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryParametersListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var parametersCategory = db.ProductCategoryParameter.Where(pcp => pcp.ProductCategoryId == request.ProductCategoryId);
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
        var parameter = await db.ProductCategoryParameter.FindAsync([id], cancellationToken) 
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));

        var values = await db.ProductCategoryParameterValue
            .Where(v => v.ProductCategoryParameterId == parameter.Id)
            .Select(v => new ProductCategoryParameterValueListItem(parameter.Id, parameter.Name, v.Value))
            .ToListAsync(cancellationToken);

        return new ProductCategoryParameterDetailResponse(
            parameter.Id,
            parameter.Name,
            parameter.ProductCategoryId,
            parameter.ProductCategory.Name,
            values);
    }

    public async Task<ProductCategoryParameterValuesListResponse> GetParameterValues(ProductCategoryParameterValuesListRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryParameterValuesListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var valuesForConditions = db.ProductCategoryParameterValue.Where(pcpv =>
            pcpv.ProductCategoryParameterId == request.ProductCategoryParameterId
            && pcpv.Value.Contains(request.Search)
            || pcpv.ProductCategoryParameter.Name.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.Search))
            valuesForConditions = valuesForConditions.Where(pcpv => 
                pcpv.Value.Contains(request.Search)
                || pcpv.ProductCategoryParameter.Name.Contains(request.Search));

        var countValuesParameter = await valuesForConditions.CountAsync(cancellationToken);

        if (request.Page != null)
            valuesForConditions = valuesForConditions.GetPage(request.Page);

        var productsItems = valuesForConditions.Select(v => new ProductCategoryParameterValueListItem(v.ProductCategoryParameterId, v.ProductCategoryParameter.Name, v.Value)).ToList();

        return new ProductCategoryParameterValuesListResponse(productsItems, new PageResponse(countValuesParameter, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<BaseResponse> Create(ProductCategoryParameterCreateRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryParameterCreateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var parameter = new ProductCategoryParameter(request.Name, request.ProductCategoryId);
        await db.ProductCategoryParameter.AddAsync(parameter, cancellationToken);

        var values = request.Values
            .Select(v => new ProductCategoryParameterValue(v, parameter.Id))
            .ToList();
        if (values.Count > 0)
            await db.ProductCategoryParameterValue.AddRangeAsync(values, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Update(ProductCategoryParameterUpdateRequest request, CancellationToken cancellationToken = default)
    {
        await new ProductCategoryParameterUpdateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var parameter = await db.ProductCategoryParameter.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));

        parameter.Name = request.Name;
        parameter.ProductCategoryId = request.ProductCategoryId;

        var valuesOnDelet = await db.ProductCategoryParameterValue
            .Where(v => 
                v.ProductCategoryParameterId == parameter.Id 
                && !request.Values.Any(value => value == v.Value))
            .ToListAsync(cancellationToken);
        if (valuesOnDelet.Count > 0)
            db.ProductCategoryParameterValue.RemoveRange(valuesOnDelet);

        var valuesOnCreate = await db.ProductCategoryParameterValue
            .Where(v =>
                v.ProductCategoryParameterId == parameter.Id
                && request.Values.Any(value => value == v.Value))
            .ToListAsync(cancellationToken);
        if (valuesOnCreate.Count > 0)
            await db.ProductCategoryParameterValue.AddRangeAsync(valuesOnCreate, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        var rowsRemoved = await db.ProductCategoryParameter.Where(pcp => pcp.Id == id).ExecuteDeleteAsync(cancellationToken);
        if (rowsRemoved == 0)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));
        return new BaseResponse();
    }
}