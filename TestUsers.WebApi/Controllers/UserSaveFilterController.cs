using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

public class UserSaveFilterController(IUserSaveFilterService _userSaveFilterService) : BaseController
{
    [HttpGet("[controller]s/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserSaveFilterListItem>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> GetList(int userId, CancellationToken cancellationToken = default)
    {
        return Ok(await _userSaveFilterService.GetList(userId, cancellationToken: cancellationToken));
    }

    [HttpPut("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Save(UserSaveFilterRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _userSaveFilterService.SaveFilter(request, cancellationToken: cancellationToken));
    }

    [HttpDelete("[controller]/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        return Ok(await _userSaveFilterService.Delete(id, cancellationToken: cancellationToken));
    }
}