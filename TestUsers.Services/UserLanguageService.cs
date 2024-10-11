using TestUsers.Data.Models;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.UserLanguages;
using TestUsers.Services.Dtos.Validators.UserLanguages;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services;

public class UserLanguageService(DataContext _db) : IUserLanguageService
{
    public async Task<List<UserLanguageItemResponse>> GetList(int userId, CancellationToken cancellationToken = default)
    {
        return await _db.UserLanguage
            .Where(ul => ul.UserId == userId)
            .Select(ul => 
            new UserLanguageItemResponse(
                ul.LanguageId,
                ul.Language.Code,
                ul.Language.Name,
                ul.DateLearn))
            .ToListAsync(cancellationToken);
    }

    public async Task<BaseResponse> AddLanguageToUser(AddLanguageToUser request, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var userSession = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(us => us.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (request.UserId != userSession.UserId)
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }
        await new AddLanguageToUserValidator().ValidateAndThrowAsync(request, cancellationToken);
        if (await _db.UserLanguage.AnyAsync(ul => ul.LanguageId == request.LanguageId && ul.UserId == request.UserId, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(UserLanguage.Language), nameof(UserLanguage.User)));
        var userLanguage = new UserLanguage(request.DateLearn, request.UserId, request.LanguageId);
        await _db.UserLanguage.AddAsync(userLanguage, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> SaveUserLanguages(SaveUserLanguagesRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var userSession = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(us => us.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (request.UserId != userSession.UserId)
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }

        await new SaveUserLanguagesRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        // Загружаем данные в память
        var existingUserLanguages = await _db.UserLanguage
            .Where(userLanguage => userLanguage.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        // Определяем языки для удаления
        var oldUserLanguagesDeleting = existingUserLanguages
            .Where(userLanguage => !request.Languages.Any(nul => nul.LanguageId == userLanguage.LanguageId))
            .ToList();

        if (oldUserLanguagesDeleting.Count != 0)
            _db.UserLanguage.RemoveRange(oldUserLanguagesDeleting);

        // Определяем языки для создания
        var createUserLanguages = request.Languages
            .Where(l => !existingUserLanguages.Any(userLanguage => userLanguage.LanguageId == l.LanguageId))
            .Select(l => new UserLanguage(l.DateLearn, request.UserId, l.LanguageId))
            .ToList();

        if (createUserLanguages.Count != 0)
            await _db.UserLanguage.AddRangeAsync(createUserLanguages, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new BaseResponse();
    }
}