using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Users;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class UserWsController(IUserService _userService) : BaseWsController
{
    public async Task<UsersListResponse> GetList(UsersListRequest request)
    {
        return await _userService.GetList(request);
    }

    public async Task<UserDetailResponse> GetDetail(UserDetailRequest request)
    {
        return await _userService.GetDetail(request.UserId);
    }

    public async Task<BaseResponse> Registration(UserCreateRequest request)
    {

        return await _userService.Create(request, Socket.SessionId);
    }

    public async Task<BaseResponse> Edit(UserEditRequest request)
    {
        return await _userService.Edit(request, Socket.SessionId);
    }

    public async Task<BaseResponse> Remove(UserDeleteRequest request)
    {
        return await _userService.Delete(request.UserId, Socket.SessionId);
    }
}
