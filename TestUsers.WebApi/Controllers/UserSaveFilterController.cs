using Microsoft.AspNetCore.Mvc;
using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebApi.Controllers;

[ApiController]
[Route("Api/v0.1")]
public class UserSaveFilterController(IUserSaveFilterService userSaveFilterService) : Controller
{
    [HttpGet("[controller]s/{userId:int}")]

    public async Task<IActionResult> GetList(int userId, CancellationToken cancellationToken = default)
    {
        return Ok(await userSaveFilterService.GetList(userId, cancellationToken));
    }

    [HttpPut("[controller]")]
    public async Task<IActionResult> Save(UserSaveFilterRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await userSaveFilterService.SaveFilter(request, cancellationToken));
    }

    [HttpDelete("[controller]/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        return Ok(await userSaveFilterService.Delete(id, cancellationToken));
    }
}