using TestUsers.Services.Dtos.UserIdentities;
using WebSocketControllers.Core.Factory;

namespace TestUsers.WebSocketApi.Controllers;

public abstract class BaseWsController : WsController<CurrentWsUser, int>
{

}
