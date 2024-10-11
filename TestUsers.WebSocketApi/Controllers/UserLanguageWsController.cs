using TestUsers.Services.Dtos.UserLanguages;
using TestUsers.Services.Dtos;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class UserLanguageWsController(IUserLanguageService _userLanguageService) : BaseWsController
{
    public async Task<List<UserLanguageItemResponse>> GetList(UserLanguageListRequest request)
    {
        return await _userLanguageService.GetList(request.UserId);
    }

    public async Task<BaseResponse> AddLanguageToUser(AddLanguageToUser request)
    {
        return await _userLanguageService.AddLanguageToUser(request, Socket.SessionId);
    }

    public async Task<BaseResponse> Save(SaveUserLanguagesRequest request)
    {
        return await _userLanguageService.SaveUserLanguages(request, Socket.SessionId);
    }
}
