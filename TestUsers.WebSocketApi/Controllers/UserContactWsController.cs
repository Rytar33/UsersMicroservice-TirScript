using TestUsers.Services.Dtos.UserContacts;
using TestUsers.Services.Interfaces.Services;
using TestUsers.WebSocketApi.Models;

namespace TestUsers.WebSocketApi.Controllers;

public class UserContactWsController(IUserContactService userContactService) : BaseWsController
{
    public async Task<List<UserContactItem>> GetList(UserContactListRequest request)
    {
        return await userContactService.GetContacts(request.UserContactId);
    }

    public async Task<bool> Save(UserContactsSaveRequest request)
    {
        await userContactService.SaveContacts(request, Socket.SessionId);
        return true;
    }
}