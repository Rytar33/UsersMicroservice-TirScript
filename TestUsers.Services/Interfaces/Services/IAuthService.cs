using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Auths;

namespace TestUsers.Services.Interfaces.Services;

public interface IAuthService
{
    Task<AuthLoginResponse> Login(AuthLoginRequest request, Guid sessionId, CancellationToken cancellationToken = default);

    Task<BaseResponse> Logout(Guid sessionId, CancellationToken cancellationToken = default);
}