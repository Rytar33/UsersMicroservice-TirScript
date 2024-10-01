using AutoMapper;
using Models;
using Models.Exceptions;
using Models.Extensions;
using Models.Validations;
using Services.Dtos;
using Services.Dtos.Pages;
using Services.Dtos.ProductCategoryParameters;
using Services.Dtos.Validators.ProductCategoryParameters;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services;

public class ProductCategoryParametersService(
    IMapper mapper,
    IProductCategoryRepository productCategoryRepository,
    IProductCategoryParameterRepository productCategoryParameterRepository,
    IProductCategoryParameterValueRepository productCategoryParameterValueRepository) : IProductCategoryParametersService
{
    public async Task<List<ProductCategoryParameterListItem>> GetList(ProductCategoryParametersListRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductCategoryParametersListRequestValidator().ValidateWithErrors(request);
        var category = await productCategoryRepository.GetByExpression(
            pc => pc.Id == request.ProductCategoryId,
            cancellationToken,
            pc => pc.Parameters);
        if (category == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategory)));
        var parametersCategory = category.Parameters.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(request.Search))
            parametersCategory = parametersCategory.Where(pcp => 
                pcp.Name.Contains(request.Search)
                || pcp.ProductCategory.Name.Contains(request.Search));
        category.Parameters = parametersCategory.ToList();
        return mapper.Map<ProductCategory, List<ProductCategoryParameterListItem>>(category);
    }

    public async Task<ProductCategoryParameterDetailResponse> GetDetail(int id, CancellationToken cancellationToken = default)
    {
        var parameter = await productCategoryParameterRepository.GetByExpression(
            pcp => pcp.Id == id,
            cancellationToken,
            pcp => pcp.ProductCategory,
            pcp => pcp.Values);
        if (parameter == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));
        return mapper.Map<ProductCategoryParameter, ProductCategoryParameterDetailResponse>(parameter);
    }

    public async Task<ProductCategoryParameterValuesListResponse> GetParameterValues(ProductCategoryParameterValuesListRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductCategoryParameterValuesListRequestValidator().ValidateWithErrors(request);
        var valuesParameter = await productCategoryParameterValueRepository.GetListByExpression(pcpv => 
            pcpv.ProductCategoryParameterId == request.ProductCategoryParameterId
            && pcpv.Value.Contains(request.Search)
            || pcpv.ProductCategoryParameter.Name.Contains(request.Search),
            cancellationToken,
            pcpv => pcpv.ProductCategoryParameter);
        var valuesForConditions = valuesParameter.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            valuesForConditions = valuesForConditions.Where(pcpv => 
                pcpv.Value.Contains(request.Search)
                || pcpv.ProductCategoryParameter.Name.Contains(request.Search));

        var countValuesParameter = valuesForConditions.Count();

        if (request.Page != null)
            valuesForConditions = valuesForConditions
                .Skip((request.Page.Page - 1) * request.Page.PageSize)
                .Take(request.Page.PageSize);

        var productsItems = valuesForConditions.AsEnumerable().Select(mapper.Map<ProductCategoryParameterValue, ProductCategoryParameterValueListItem>).ToList();

        return new ProductCategoryParameterValuesListResponse(productsItems, new PageResponse(countValuesParameter, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<BaseResponse> Create(ProductCategoryParameterCreateRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductCategoryParameterCreateRequestValidator().ValidateWithErrors(request);
        var productCategory = await productCategoryRepository.GetByExpression(pc => pc.Id == request.ProductCategoryId, cancellationToken, p => p.Products);
        if (productCategory == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategory)));

        try
        {
            await productCategoryParameterRepository.StartTransaction(cancellationToken);

            var productCategoryParameter =
                mapper.Map<ProductCategoryParameterCreateRequest, ProductCategoryParameter>(request);

            await productCategoryParameterRepository.CreateAsync(productCategoryParameter, cancellationToken);
            await productCategoryParameterRepository.SaveChangesAsync(cancellationToken);

            foreach (var parameterValue in request.Values.Select(value => new ProductCategoryParameterValue(value, productCategoryParameter.Id)))
            {
                await productCategoryParameterValueRepository.CreateAsync(parameterValue, cancellationToken);
                await productCategoryParameterValueRepository.SaveChangesAsync(cancellationToken);
            }

            await productCategoryParameterRepository.CommitTransaction(cancellationToken);
            return new BaseResponse();
        }
        catch (Exception)
        {
            await productCategoryParameterRepository.RollBackTransaction(cancellationToken);
            throw;
        }
    }

    public async Task<BaseResponse> Update(ProductCategoryParameterUpdateRequest request, CancellationToken cancellationToken = default)
    {
        _ = new ProductCategoryParameterUpdateRequestValidator().ValidateWithErrors(request);

        var parameter = await productCategoryParameterRepository.GetByExpression(
            pcp => pcp.Id == request.Id,
            cancellationToken,
            pcp => pcp.Values,
            pcp => pcp.ProductCategory,
            pcp => pcp.ProductCategory.Products);
        if (parameter == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));

        try
        {
            await productCategoryParameterRepository.StartTransaction(cancellationToken);

            var productCategoryParameter =
                mapper.Map<ProductCategoryParameterUpdateRequest, ProductCategoryParameter>(request);

            await productCategoryParameterRepository.UpdateAsync(productCategoryParameter, cancellationToken);
            await productCategoryParameterRepository.SaveChangesAsync(cancellationToken);

            foreach (var parameterValue in request.Values.Select(value => new ProductCategoryParameterValue(value, productCategoryParameter.Id)))
            {
                var getParameterValue = await productCategoryParameterValueRepository.GetByExpression(
                    pcpv => pcpv.Value == parameterValue.Value,
                    cancellationToken);
                if (getParameterValue != null) 
                    continue;

                await productCategoryParameterValueRepository.CreateAsync(parameterValue, cancellationToken);
                await productCategoryParameterValueRepository.SaveChangesAsync(cancellationToken);
            }

            foreach (var value in productCategoryParameter.Values.Where(value => request.Values.All(v => v != value.Value)))
            {
                productCategoryParameterValueRepository.Delete(value);
                await productCategoryParameterValueRepository.SaveChangesAsync(cancellationToken);
            }

            await productCategoryParameterRepository.CommitTransaction(cancellationToken);
            return new BaseResponse();
        }
        catch (Exception)
        {
            await productCategoryParameterRepository.RollBackTransaction(cancellationToken);
            throw;
        }
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        var parameter = await productCategoryParameterRepository.GetByExpression(
            pcp => pcp.Id == id,
            cancellationToken);
        if (parameter == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));

        productCategoryParameterRepository.Delete(parameter);
        await productCategoryParameterRepository.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }
}