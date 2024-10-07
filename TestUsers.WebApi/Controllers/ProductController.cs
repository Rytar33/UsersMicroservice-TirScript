using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Products;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

[ApiController]
[Route("Api/v0.1")]
public class ProductController(IProductService productService) : Controller
{
    [HttpGet("[controller]s")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductListResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> GetList([FromQuery] ProductListRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await productService.GetList(request, cancellationToken));
    }

    [HttpGet("[controller]/{productId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDetailResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> GetDetail(int productId, CancellationToken cancellationToken = default)
    {
        return Ok(await productService.GetDetail(productId, cancellationToken));
    }

    [HttpPut("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> Save(ProductSaveRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await productService.Save(request, cancellationToken));
    }

    [HttpDelete("[controller]/{productId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Delete(int productId, CancellationToken cancellationToken = default)
    {
        return Ok(await productService.Delete(productId, cancellationToken));
    }
}