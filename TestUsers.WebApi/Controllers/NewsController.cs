using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.News;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

[Route("News")]
public class NewsController(INewsService newsService) : BaseController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NewsListResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> GetList([FromQuery] NewsListRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await newsService.GetList(request, cancellationToken));
    }

    [HttpGet("{newsId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NewsDetailResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> GetDetail(int newsId, CancellationToken cancellationToken = default)
    {
        return Ok(await newsService.GetDetail(newsId, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Create(NewsCreateRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await newsService.Create(request, cancellationToken: cancellationToken));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Edit(NewsEditRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await newsService.Edit(request, cancellationToken: cancellationToken));
    }

    [HttpDelete("{newsId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Delete(int newsId, CancellationToken cancellationToken = default)
    {
        await newsService.Delete(newsId, cancellationToken: cancellationToken);
        return NoContent();
    }
}
