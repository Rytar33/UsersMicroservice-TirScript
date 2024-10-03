using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestUsers.Data;
using TestUsers.Data.Models;
using TestUsers.Services.Dtos;
using TestUsers.Services.Dtos.UserSaveFilters;
using TestUsers.Services.Exceptions;
using TestUsers.Services.Interfaces.Services;

namespace TestUsers.Services;

public class UserSaveFilterService(DataContext db) : IUserSaveFilterService
{
    public async Task<List<UserSaveFilterListItem<TJsonFilter>>> GetList<TJsonFilter>(int userId, CancellationToken cancellationToken = default) where TJsonFilter : class
    {
        if (!await db.User.AnyAsync(u => u.Id == userId, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));

        var userFilters = await db.UserSaveFilter
            .Where(usf => usf.UserId == userId)
            .ToListAsync(cancellationToken);

        return userFilters.Select(uf =>
                new UserSaveFilterListItem<TJsonFilter>(
                    uf.Id,
                    uf.UserId,
                    uf.FilterName,
                    JsonConvert.DeserializeObject<TJsonFilter>(uf.FilterValueJson)
                        ?? throw new JsonDeserializeException(string.Format(ErrorMessages.JsonDeserializeError, nameof(TJsonFilter))),
                    uf.DateCreated)).ToList();
    }

    public async Task<BaseResponse> SaveFilter<TJsonFilter>(UserSaveFilterRequest<TJsonFilter> request, CancellationToken cancellationToken = default) where TJsonFilter : class
    {
        if (!await db.User.AnyAsync(u => u.Id == request.UserId, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        if(await db.UserSaveFilter.AnyAsync(usf => usf.UserId == request.UserId && usf.FilterName == request.SaveFilterName, cancellationToken))
        {
            var userSaveFilter = await db.UserSaveFilter.FirstOrDefaultAsync(usf => usf.UserId == request.UserId && usf.FilterName == request.SaveFilterName, cancellationToken)
                ?? throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserSaveFilter)));
            userSaveFilter.FilterValueJson = JsonConvert.SerializeObject(request.SaveFilterValue);
        }
        else
        {
            var userSaveFilter = new UserSaveFilter(
                request.UserId,
                request.SaveFilterName,
                JsonConvert.SerializeObject(request.SaveFilterValue),
                DateTime.UtcNow);
            await db.UserSaveFilter.AddAsync(userSaveFilter, cancellationToken);
        }
        await db.SaveChangesAsync(cancellationToken);
        return new BaseResponse();
    }

    public async Task<BaseResponse> Delete(int id, CancellationToken cancellationToken = default)
    {
        var rowsRemoved = await db.UserSaveFilter.Where(usf => usf.Id == id).ExecuteDeleteAsync(cancellationToken);
        if (rowsRemoved == 0)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserSaveFilter)));
        return new BaseResponse();
    }
}