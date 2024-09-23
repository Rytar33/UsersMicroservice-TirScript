using System.ComponentModel.DataAnnotations;
using Models;
using Services.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

/// <summary>
/// Реализация репозитория для таблицы пользователя
/// </summary>
/// <param name="dataContext">Контекст базы данных</param>
public class UserRepository(DataContext dataContext) : IUserRepository
{
    public async Task<List<User>> GetListByExpression(
       Expression<Func<User, bool>>? expression = null, 
       CancellationToken cancellationToken = default)
    {
        var users = dataContext.User.AsNoTracking();
        if (expression != null)
            users = users.Where(expression);
        return await users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByExpression(
        Expression<Func<User, bool>> expression,
        CancellationToken cancellationToken = default)
        => await dataContext.User.AsNoTracking().FirstOrDefaultAsync(expression, cancellationToken);

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (await dataContext.User.AnyAsync(u => u.Email == user.Email, cancellationToken))
            throw new ValidationException(string.Format(ErrorMessages.CoincideError, nameof(User.Email), nameof(User)));
        await dataContext.User.AddAsync(user, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var userById = await GetByExpression(u => u.Id == user.Id, cancellationToken);
        if (userById == null)
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(User)));
        dataContext.User.Update(user);
    }

    public void Delete(User user)
        => dataContext.Remove(user);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);
}