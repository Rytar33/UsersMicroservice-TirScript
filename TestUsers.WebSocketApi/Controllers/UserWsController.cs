using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Users;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class UserWsController(IUserService userService) : BaseWsController
{
    public async Task<UsersListResponse> GetList(UsersListRequest request)
    {
        return await userService.GetList(request);
    }

    public async Task<UserDetailResponse> GetDetail(UserDetailRequest request)
    {
        return await userService.GetDetail(request.UserId);
    }

    public async Task<BaseResponse> Registration(UserCreateRequest request)
    {

        return await userService.Create(request, Socket.SessionId);
    }

    public async Task<BaseResponse> Edit(UserEditRequest request)
    {
        return await userService.Edit(request, Socket.SessionId);
    }

    public async Task<BaseResponse> Remove(UserDeleteRequest request)
    {
        return await userService.Delete(request.UserId, Socket.SessionId);
    }
}
