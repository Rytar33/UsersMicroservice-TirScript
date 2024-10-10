using TestUsers.Data.Models;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.Pages;
using TestUsers.Services.Dtos.Users;
using TestUsers.Services.Dtos.Users.Recoveries;
using TestUsers.Services.Extensions;
using TestUsers.Services.Dtos.Validators.Users;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Services.Exceptions;
using TestUsers.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TestUsers.Services;

public class UserService(IEmailService emailService, DataContext db) : IUserService
{
    public async Task<UsersListResponse> GetList(UsersListRequest request, CancellationToken cancellationToken = default)
    {
        await new UsersListRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var usersForConditions = db.User.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            usersForConditions = usersForConditions
                .Where(x => x.FullName.Contains(request.Search) || x.Email.Contains(request.Search));

        if (request.Status != null)
            usersForConditions = usersForConditions.Where(x => x.Status == request.Status);

        var countUsers = await usersForConditions.CountAsync(cancellationToken);

        if (request.Page != null)
            usersForConditions = usersForConditions.GetPage(request.Page);

        var usersItems = await usersForConditions.Select(u => new UsersListItem(u.Id, u.Email, u.FullName, u.DateRegister, u.Status)).ToListAsync(cancellationToken);

        return new UsersListResponse(usersItems, new PageResponse(countUsers, request.Page?.Page ?? 0, request.Page?.PageSize ?? 0));
    }

    public async Task<UserDetailResponse> GetDetail(int userId, CancellationToken cancellationToken = default)
    {
        var user = await db.User.FindAsync([ userId ], cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        return new UserDetailResponse(user.Id, user.Email, user.FullName, user.DateRegister, user.Status);
    }

    public async Task<BaseResponse> Create(UserCreateRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            if (await db.UserSession.AnyAsync(us => us.SessionId == sessionId, cancellationToken))
                throw new IsAuthException(ErrorMessages.YouAuthError);
        }
        await new UserCreateRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        if (await db.User.AnyAsync(u => u.Email == request.Email, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(User.Email), nameof(User)));
        if (await db.User.AnyAsync(u => u.FullName == request.FullName, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(User.FullName), nameof(User)));
        var user = new User(request.Email, request.FullName, request.Password.GetSha256());
        await db.User.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Edit(UserEditRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        await new UserEditRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        var user = await db.User.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        if (await db.User.AnyAsync(u => user.Id != u.Id && u.FullName == request.FullName, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(User.FullName), nameof(User)));
        if (sessionId.HasValue)
        {
            var userSession = await db.UserSession.AsNoTracking().FirstOrDefaultAsync(u => u.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (user.Id != userSession.UserId)
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }
        user.FullName = request.FullName;
        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int userId, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var userSession = await db.UserSession.AsNoTracking().FirstOrDefaultAsync(us => us.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (userSession.UserId != userId)
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }
        if (!db.Database.IsInMemory())
        {
            var rowsRemoved = await db.User.Where(u => u.Id == userId).ExecuteDeleteAsync(cancellationToken);
            if (rowsRemoved == 0)
                throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        }
        else
        {
            var user = await db.User.FindAsync([ userId ], cancellationToken) 
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
            db.User.Remove(user);
            await db.SaveChangesAsync(cancellationToken);
        }
        return new BaseResponse();
    }

    public async Task<BaseResponse> RecoveryStart(RecoveryStartRequest request, CancellationToken cancellationToken = default)
    {
        await new RecoveryStartRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var user = await db.User.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken) 
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        if (request.RequestCode == null)
        {
            user.RecoveryToken = string.Empty.GetGenerateToken();
            await db.SaveChangesAsync(cancellationToken);
            if (!db.Database.IsInMemory())
                await emailService.SendEmailAsync(user.Email, cancellationToken);
        }
        else
        {
            if (user.RecoveryToken != request.RequestCode)
                throw new ArgumentException(string.Format(ErrorMessages.NotCoincideError, nameof(User.RecoveryToken)));
        }

        return new BaseResponse();
    }

    public async Task<BaseResponse> RecoveryEnd(RecoveryEndRequest request, CancellationToken cancellationToken = default)
    {
        await new RecoveryEndRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var user = await db.User.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken) 
            ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        if (user.RecoveryToken != request.RecoveryTokenRequest)
            throw new ArgumentException(string.Format(ErrorMessages.NotCoincideError, nameof(User.RecoveryToken)));

        user.PasswordHash = request.NewPassword.GetSha256();
        user.RecoveryToken = null;

        await db.SaveChangesAsync(cancellationToken);

        return new BaseResponse();
    }
}