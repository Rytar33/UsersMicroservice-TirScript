using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Users;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

public class UserController(IUserService userService) : BaseController
{
    [HttpGet("[controller]s")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsersListResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> GetList([FromQuery] UsersListRequest request, CancellationToken cancellationToken)
    {
        return Ok(await userService.GetList(request, cancellationToken));
    }

    [HttpGet("[controller]/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDetailResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> GetDetail(int userId, CancellationToken cancellationToken)
    {
        return Ok(await userService.GetDetail(userId, cancellationToken));
    }

    [HttpPost("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    public async Task<IActionResult> Registration(UserCreateRequest request, CancellationToken cancellationToken = default)
    {
        
        return Ok(await userService.Create(request, cancellationToken: cancellationToken));
    }

    [HttpPut("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Edit(UserEditRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await userService.Edit(request, cancellationToken: cancellationToken));
    }

    [HttpDelete("[controller]/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Remove(int userId, CancellationToken cancellationToken = default)
    {
        return Ok(await userService.Delete(userId, cancellationToken: cancellationToken));
    }
}