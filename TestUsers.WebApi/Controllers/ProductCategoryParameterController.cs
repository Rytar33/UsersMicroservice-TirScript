﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.ProductCategoryParameters;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

public class ProductCategoryParameterController(IProductCategoryParametersService _productCategoryParametersService) : BaseController
{
    [HttpGet("[controller]s")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductCategoryParameterListItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> GetList([FromQuery] ProductCategoryParametersListRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryParametersService.GetList(request, cancellationToken));
    }

    [HttpGet("[controller]/Values")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductCategoryParameterValuesListResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> GetParameterValues([FromQuery] ProductCategoryParameterValuesListRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryParametersService.GetParameterValues(request, cancellationToken));
    }

    [HttpGet("[controller]/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductCategoryParameterDetailResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> GetDetail(int id, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryParametersService.GetDetail(id, cancellationToken));
    }

    [HttpPost("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> Create(ProductCategoryParameterCreateRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryParametersService.Create(request, cancellationToken));
    }

    [HttpPut("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Update(ProductCategoryParameterUpdateRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryParametersService.Update(request, cancellationToken));
    }

    [HttpDelete("[controller]/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryParametersService.Delete(id, cancellationToken));
    }
}