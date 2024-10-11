using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Dtos;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class UserSaveFilterWsController(IUserSaveFilterService _userSaveFilterService) : BaseWsController
{
    public async Task<List<UserSaveFilterListItem>> GetList(UserSaveFilterListRequest request)
    {
        return await _userSaveFilterService.GetList(request.UserId, Socket.SessionId);
    }

    public async Task<BaseResponse> Save(UserSaveFilterRequest request)
    {
        return await _userSaveFilterService.SaveFilter(request, Socket.SessionId);
    }

    public async Task<BaseResponse> Delete(UserSaveFilterDeleteRequest request)
    {
        return await _userSaveFilterService.Delete(request.Id, Socket.SessionId);
    }
}
