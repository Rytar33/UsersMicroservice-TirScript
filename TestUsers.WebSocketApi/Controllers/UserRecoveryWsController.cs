using TestUsers.Services.Dtos.Users.Recoveries;
using TestUsers.Services.Dtos;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class UserRecoveryWsController(IUserService _userService) : BaseWsController
{
    public async Task<BaseResponse> SendCode(RecoverySendCodeRequest request)
    {
        return await _userService.RecoveryStart(new RecoveryStartRequest(request.Email, null));
    }

    public async Task<BaseResponse> ConfrimCode(RecoveryStartRequest request)
    {
        return await _userService.RecoveryStart(request);
    }

    public async Task<BaseResponse> RecoveryEnd(RecoveryEndRequest request)
    {
        return await _userService.RecoveryEnd(request);
    }
}