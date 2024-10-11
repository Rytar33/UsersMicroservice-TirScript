using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.UserLanguages;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

[Route("[controller]")]
public class UserLanguageController(IUserLanguageService _userLanguageService) : BaseController
{
    [HttpGet("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserLanguageItemResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> GetList(int userId, CancellationToken cancellationToken = default)
    {
        return Ok(await _userLanguageService.GetList(userId, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> AddLanguageToUser(AddLanguageToUser request, CancellationToken cancellationToken = default)
    {
        return Ok(await _userLanguageService.AddLanguageToUser(request, cancellationToken: cancellationToken));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse))]
    public async Task<IActionResult> Save(SaveUserLanguagesRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _userLanguageService.SaveUserLanguages(request, cancellationToken: cancellationToken));
    }
}