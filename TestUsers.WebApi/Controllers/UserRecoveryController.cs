using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Users.Recoveries;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

[ApiController]
[Route("Api/v0.1/[controller]/[action]")]
public class UserRecoveryController(IUserService userService) : Controller
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> SendCode(string email, CancellationToken cancellationToken = default)
    {
        return Ok(await userService.RecoveryStart(new RecoveryStartRequest(email, null), cancellationToken));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> ConfrimCode(RecoveryStartRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await userService.RecoveryStart(request, cancellationToken));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> RecoveryEnd(RecoveryEndRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await userService.RecoveryEnd(request, cancellationToken));
    }
}