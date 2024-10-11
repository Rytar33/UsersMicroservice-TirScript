using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.UserContacts;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

[Route("[controller]s")]
public class UserContactController(IUserContactService _userContactService) : BaseController
{
    [HttpGet("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserContactItem>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> GetList(int userId, CancellationToken cancellationToken = default)
    {
        return Ok(await _userContactService.GetContacts(userId, cancellationToken));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Save(UserContactsSaveRequest request, CancellationToken cancellationToken = default)
    {
        await _userContactService.SaveContacts(request, cancellationToken: cancellationToken);
        return NoContent();
    }
}