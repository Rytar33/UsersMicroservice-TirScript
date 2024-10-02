using TestUsers.Data.Models;
using TestUsers.Services.Extensions;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.UserLanguages;
using TestUsers.Services.Dtos.Validators.UserLanguages;
using TestUsers.Services.Interfaces.Services;
using TestUsers.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using TestUsers.Services.Exceptions;

namespace TestUsers.Services;

public class UserLanguageService(DataContext db) : IUserLanguageService
{
    public async Task<List<UserLanguageItemResponse>> GetList(int userId, CancellationToken cancellationToken = default)
    {
        return await db.UserLanguage
            .Where(ul => ul.UserId == userId)
            .Select(ul => 
            new UserLanguageItemResponse(
                ul.LanguageId,
                ul.Language.Code,
                ul.Language.Name,
                ul.DateLearn))
            .ToListAsync(cancellationToken);
    }

    public async Task<BaseResponse> AddLanguageToUser(AddLanguageToUser request, CancellationToken cancellationToken = default)
    {
        await new AddLanguageToUserValidator().ValidateAndThrowAsync(request, cancellationToken);
        if (await db.UserLanguage.AnyAsync(ul => ul.LanguageId == request.LanguageId && ul.UserId == request.UserId, cancellationToken))
            throw new ConcidedException(string.Format(ErrorMessages.CoincideError, nameof(UserLanguage.Language), nameof(UserLanguage.User)));
        var userLanguage = new UserLanguage(request.DateLearn, request.UserId, request.LanguageId);
        await db.UserLanguage.AddAsync(userLanguage, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> SaveUserLanguages(SaveUserLanguagesRequest request, CancellationToken cancellationToken = default)
    {
        await new SaveUserLanguagesRequestValidator().ValidateAndThrowAsync(request, cancellationToken);

        var oldUserLanguagesDeleting = await db.UserLanguage.Where(userLanguage => !request.Languages.Any(nul =>
            userLanguage.UserId == request.UserId
            && nul.LanguageId == userLanguage.LanguageId)).ToListAsync(cancellationToken);

        if (oldUserLanguagesDeleting.Count != 0)
            db.UserLanguage.RemoveRange(oldUserLanguagesDeleting);

        var createUserLanguages = request.Languages.Select(l => new UserLanguage(l.DateLearn, request.UserId, l.LanguageId)).ToList();

        if (createUserLanguages.Count != 0)
            await db.UserLanguage.AddRangeAsync(createUserLanguages, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return new BaseResponse();
    }
}