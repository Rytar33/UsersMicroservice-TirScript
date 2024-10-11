using TestUsers.Services.Dtos.UserContacts;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class UserContactWsController(IUserContactService _userContactService) : BaseWsController
{
    public async Task<List<UserContactItem>> GetList(UserContactListRequest request)
    {
        return await _userContactService.GetContacts(request.UserContactId);
    }

    public async Task<bool> Save(UserContactsSaveRequest request)
    {
        await _userContactService.SaveContacts(request, Socket.SessionId);
        return true;
    }
}