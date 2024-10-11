using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Dtos.Validators.UserSaveFilters;
using TestUsers.Services.Exceptions;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.Services;

public class UserSaveFilterService(DataContext _db) : IUserSaveFilterService
{
    public async Task<List<UserSaveFilterListItem>> GetList(int userId, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var userSession = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(us => us.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (userId != userSession.UserId)
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }

        if (!await _db.User.AnyAsync(u => u.Id == userId, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));

        var userFilters = await _db.UserSaveFilter
            .Where(usf => usf.UserId == userId)
            .ToListAsync(cancellationToken);

        var valuesIds = await _db.UserSaveFilterRelation
            .Where(usfr => usfr.UserSaveFilterId == userId)
            .Select(usfr => usfr.ProductCategoryParameterValueId)
            .ToListAsync(cancellationToken);

        return userFilters.Select(uf =>
                new UserSaveFilterListItem(
                    uf.Id,
                    uf.UserId,
                    uf.FilterName,
                    valuesIds,
                    uf.CategoryId,
                    uf.Search,
                    uf.FromAmount,
                    uf.ToAmount,
                    uf.DateCreated)).ToList();
    }

    public async Task<BaseResponse> SaveFilter(UserSaveFilterRequest request, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var userSession = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(us => us.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (request.UserId != userSession.UserId)
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }
        await new UserSaveFilterRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        if (!await _db.User.AnyAsync(u => u.Id == request.UserId, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));

        // On create
        if(await _db.UserSaveFilter.AnyAsync(usf => usf.UserId == request.UserId && usf.FilterName == request.SaveFilterName, cancellationToken))
        {
            var userSaveFilter = await _db.UserSaveFilter.FirstOrDefaultAsync(usf => usf.UserId == request.UserId && usf.FilterName == request.SaveFilterName, cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserSaveFilter)));
            
            userSaveFilter.CategoryId = request.CategoryId;
            userSaveFilter.Search = request.Search;
            userSaveFilter.FromAmount = request.FromAmount;
            userSaveFilter.ToAmount = request.ToAmount;

            if (request.CategoryParametersValuesIds != null)
            {
                var valuesRelationOnDelete = await _db.UserSaveFilterRelation
                .Where(usfr => !request.CategoryParametersValuesIds.Contains(usfr.ProductCategoryParameterValueId))
                .ToListAsync(cancellationToken);

                if (valuesRelationOnDelete.Count > 0)
                    _db.UserSaveFilterRelation.RemoveRange(valuesRelationOnDelete);

                var valuesRelationOnCreate = request.CategoryParametersValuesIds
                    .Distinct()
                    .Where(i => !_db.UserSaveFilterRelation.Any(usfr =>
                        usfr.ProductCategoryParameterValueId == i
                        && usfr.UserSaveFilterId == userSaveFilter.Id))
                    .Select(i => new UserSaveFilterRelation(i, userSaveFilter.Id))
                    .ToList();

                if (valuesRelationOnCreate.Count > 0)
                    await _db.UserSaveFilterRelation.AddRangeAsync(valuesRelationOnCreate, cancellationToken);
            }
        }
        else // On update
        {
            var userSaveFilter = new UserSaveFilter(
                request.UserId,
                request.SaveFilterName,
                DateTime.UtcNow,
                request.CategoryId,
                request.Search,
                request.FromAmount,
                request.ToAmount);

            await _db.UserSaveFilter.AddAsync(userSaveFilter, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            if (request.CategoryParametersValuesIds != null)
            {
                var userSaveFilterRelations = request.CategoryParametersValuesIds
                .Select(i => new UserSaveFilterRelation(i, userSaveFilter.Id))
                .ToList();

                if (userSaveFilterRelations.Count > 0)
                    await _db.UserSaveFilterRelation.AddRangeAsync(userSaveFilterRelations, cancellationToken);
            }
        }
        await _db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int id, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        if (sessionId.HasValue)
        {
            var userSession = await _db.UserSession.AsNoTracking().FirstOrDefaultAsync(us => us.SessionId == sessionId, cancellationToken)
                ?? throw new UnAuthorizedException(ErrorMessages.UnAuthError);
            if (!await _db.UserSaveFilter.AnyAsync(usf => usf.Id == id && userSession.UserId == usf.UserId, cancellationToken))
                throw new ForbiddenException(ErrorMessages.ForbiddenError);
        }
        if (!_db.Database.IsInMemory())
        {
            var rowsRemoved = await _db.UserSaveFilter.Where(u => u.Id == id).ExecuteDeleteAsync(cancellationToken);
            if (rowsRemoved == 0)
                throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserSaveFilter)));
        }
        else
        {
            var userSaveFilter = await _db.UserSaveFilter.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserSaveFilter)));
            _db.UserSaveFilter.Remove(userSaveFilter);
            await _db.SaveChangesAsync(cancellationToken);
        }
        return new BaseResponse();
    }
}