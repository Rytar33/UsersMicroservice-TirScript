using Microsoft.EntityFrameworkCore;
using TestUsers.Data;
using TestUsers.Services.Dtos.UserIdentities;
using Users.IIdentityService;
using Users.IIdentityService.Models;

namespace TestUsers.Services;

public class UserIdentityService(DataContext dataContext) : IUsersIdentityService<CurrentWsUser, int>
{
    public async Task<UserBySessionIdResponse<CurrentWsUser, int>> GetUserBySessionId(Guid sessionId, HttpRequestInfo? requestInfo = null)
    {
        var userSession = await dataContext.UserSession.AsNoTracking().FirstOrDefaultAsync(us => us.SessionId == sessionId);
        if (userSession == null)
            return new UserBySessionIdResponse<CurrentWsUser, int>(new CurrentWsUser
            {
                IsAuthorized = false
            });
        return new UserBySessionIdResponse<CurrentWsUser, int>(new CurrentWsUser
        {
            Id = userSession.UserId,
            IsAuthorized = true
        });
    }
}