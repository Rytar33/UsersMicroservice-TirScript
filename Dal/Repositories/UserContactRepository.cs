using System.Data;
using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

public class UserContactRepository(DataContext dataContext) : IUserContactRepository
{
    public async Task<List<UserContact>> GetListByExpression(
        Expression<Func<UserContact, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<UserContact, object>>[] includes)
    {
        var usersContacts = dataContext.UserContact.AsNoTracking();
        if (expression != null)
            usersContacts = usersContacts.Where(expression);
        Array.ForEach(includes, i => usersContacts = usersContacts.Include(i));
        return await usersContacts.ToListAsync(cancellationToken);
    }

    public async Task<UserContact?> GetByExpression(
        Expression<Func<UserContact, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<UserContact, object>>[] includes)
    {
        var usersContacts = dataContext.UserContact.AsNoTracking();
        Array.ForEach(includes, i => usersContacts = usersContacts.Include(i));
        return await usersContacts.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task CreateAsync(UserContact userContact, CancellationToken cancellationToken = default)
    {
        if (await dataContext.UserContact.AnyAsync(uc =>
                uc.Id == userContact.UserId
                && uc.Name == userContact.Name,
                cancellationToken))
            throw new DuplicateNameException(string.Format(ErrorMessages.CoincideError, nameof(UserContact.Name), nameof(User)));
        await dataContext.UserContact.AddAsync(userContact, cancellationToken);
    }

    public async Task UpdateAsync(UserContact userContact, CancellationToken cancellationToken = default)
    {
        if (!await dataContext.UserContact.AnyAsync(uc => uc.Id == userContact.Id, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(UserContact.Id)));
        dataContext.UserContact.Update(userContact);
    }

    public void Delete(UserContact userContact)
        => dataContext.UserContact.Remove(userContact);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);

    public async Task StartTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollBackTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.RollbackTransactionAsync(cancellationToken);
}