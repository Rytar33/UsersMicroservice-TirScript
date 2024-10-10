using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Auths;
using TestUsers.Services.Dtos.Validators.Auths;
using TestUsers.Services.Exceptions;
using TestUsers.Services.Extensions;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.Services;

public class AuthService(DataContext dataContext) : IAuthService
{
    public async Task<AuthLoginResponse> Login(AuthLoginRequest request, Guid sessionId, CancellationToken cancellationToken = default)
    {
        await new AuthLoginRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var user = await dataContext.User.FirstOrDefaultAsync(u =>
                u.Email == request.Email
                && u.PasswordHash == request.Password.GetSha256(),
            cancellationToken)
            ?? throw new NotConcidedException(string.Format(ErrorMessages.NotCoincideError, $"{nameof(AuthLoginRequest.Email)} и/или {nameof(AuthLoginRequest.Password)}"));

        if (await dataContext.UserSession.AnyAsync(us => sessionId == us.SessionId, cancellationToken))
            throw new IsAuthException(ErrorMessages.YouAuthError);
        await dataContext.UserSession.AddAsync(new UserSession(user.Id, sessionId), cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
        return new AuthLoginResponse(user.Id);
    }

    public async Task<BaseResponse> Logout(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var rowsRemoved = await dataContext.UserSession.Where(us => us.SessionId == sessionId).ExecuteDeleteAsync(cancellationToken);
        if (rowsRemoved == 0)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserSession)));
        return new BaseResponse();
    }
}
