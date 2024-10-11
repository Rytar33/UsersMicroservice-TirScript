using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.ProductCategory;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

public class ProductCategoryController(IProductCategoryService _productCategoryService) : BaseController
{
    [HttpGet("[controller]s")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductCategoryListItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> GetListByParent([FromQuery] ProductCategoryGetListByParentRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryService.GetListByParent(request, cancellationToken));
    }

    [HttpGet("[controller]s/Tree/{parentCategoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductCategoryTreeItem>))]
    public async Task<IActionResult> GetTreeByParent(int? parentCategoryId, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryService.GetTreeByParent(parentCategoryId, cancellationToken));
    }

    [HttpGet("[controller]s/Tree")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductCategoryTreeItem>))]
    public async Task<IActionResult> GetTree(CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryService.GetTree(cancellationToken));
    }

    [HttpPost("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> Create(ProductCategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryService.Create(request, cancellationToken));
    }

    [HttpPut("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Update(ProductCategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryService.Update(request, cancellationToken));
    }

    [HttpDelete("[controller]/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        return Ok(await _productCategoryService.Delete(id, cancellationToken));
    }
}