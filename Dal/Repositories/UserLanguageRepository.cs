using System.Data.Entity;
using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

public class UserLanguageRepository(DataContext dataContext) : IUserLanguageRepository
{
    public async Task<List<UserLanguage>> GetListByExpression(
        Expression<Func<UserLanguage, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<UserLanguage, object>>[] includes)
    {
        var usersLanguage = dataContext.UserLanguage.AsNoTracking();
        if (expression != null)
            usersLanguage = usersLanguage.Where(expression);
        Array.ForEach(includes, i => usersLanguage = usersLanguage.Include(i));
        return await usersLanguage.ToListAsync(cancellationToken);
    }

    public async Task<UserLanguage?> GetByExpression(
        Expression<Func<UserLanguage, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<UserLanguage, object>>[] includes)
    {
        var usersLanguage = dataContext.UserLanguage.AsNoTracking();
        Array.ForEach(includes, i => usersLanguage = usersLanguage.Include(i));
        return await usersLanguage.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task CreateAsync(UserLanguage userLanguage, CancellationToken cancellationToken = default)
    {
        if (await dataContext.UserLanguage.AnyAsync(ul =>
                    ul.UserId == userLanguage.UserId
                    && ul.LanguageId == userLanguage.LanguageId,
                cancellationToken))
            throw new ArgumentException(string.Format(ErrorMessages.CoincideError, nameof(UserLanguage)));
        await dataContext.UserLanguage.AddAsync(userLanguage, cancellationToken);
    }

    public async Task UpdateAsync(UserLanguage userLanguage, CancellationToken cancellationToken = default)
    {
        var oldUserLanguage = await 
            dataContext.UserLanguage
                .AsNoTracking()
                .FirstOrDefaultAsync(ul => 
                    ul.Id == userLanguage.Id,
                    cancellationToken);
        if (oldUserLanguage == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserLanguage)));
        dataContext.UserLanguage.Update(userLanguage);
    }

    public void Delete(UserLanguage userLanguage)
        => dataContext.UserLanguage.Remove(userLanguage);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);

    public async Task StartTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollBackTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.RollbackTransactionAsync(cancellationToken);
}