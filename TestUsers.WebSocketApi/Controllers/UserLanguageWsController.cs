using TestUsers.Services.Dtos.UserLanguages;
using TestUsers.Services.Dtos;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class UserLanguageWsController(IUserLanguageService userLanguageService) : BaseWsController
{
    public async Task<List<UserLanguageItemResponse>> GetList(UserLanguageListRequest request)
    {
        return await userLanguageService.GetList(request.UserId);
    }

    public async Task<BaseResponse> AddLanguageToUser(AddLanguageToUser request)
    {
        return await userLanguageService.AddLanguageToUser(request, Socket.SessionId);
    }

    public async Task<BaseResponse> Save(SaveUserLanguagesRequest request)
    {
        return await userLanguageService.SaveUserLanguages(request, Socket.SessionId);
    }
}
