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

public class AuthService(DataContext _db) : IAuthService
{
    public async Task<AuthLoginResponse> Login(AuthLoginRequest request, Guid sessionId, CancellationToken cancellationToken = default)
    {
        await new AuthLoginRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var user = await _db.User.FirstOrDefaultAsync(u =>
                u.Email == request.Email
                && u.PasswordHash == request.Password.GetSha256(),
            cancellationToken)
            ?? throw new NotConcidedException(string.Format(ErrorMessages.NotCoincideError, $"{nameof(AuthLoginRequest.Email)} и/или {nameof(AuthLoginRequest.Password)}"));

        if (await _db.UserSession.AnyAsync(us => sessionId == us.SessionId, cancellationToken))
            throw new IsAuthException(ErrorMessages.YouAuthError);
        await _db.UserSession.AddAsync(new UserSession(user.Id, sessionId), cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return new AuthLoginResponse(user.Id);
    }

    public async Task<BaseResponse> Logout(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var rowsRemoved = await _db.UserSession.Where(us => us.SessionId == sessionId).ExecuteDeleteAsync(cancellationToken);
        if (rowsRemoved == 0)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserSession)));
        return new BaseResponse();
    }
}
