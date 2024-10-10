﻿using TestUsers.Services.Dtos.Auths;
using TestUsers.Services.Dtos.UserIdentities;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.WebSocketApi.Controllers;

public class AuthWsController(IAuthService authService) : BaseWsController
{
    public CurrentWsUser? GetCurrentUser()
    {
        return User.User;
    }

    public async Task<bool> Login(AuthLoginRequest request)
    {
        var response = await authService.Login(request, User.SessionToken);
        await User.AuthorizedUser(new CurrentWsUser() { Id = response.IdUser, IsAuthorized = true });
        return true;
    }

    public async Task<bool> Logout()
    {
        await authService.Logout(Socket.SessionId);
        await User.UnauthorizedUser(Socket);
        return true;
    }
}