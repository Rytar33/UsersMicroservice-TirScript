using System.Data.Entity;
using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

public class NewsTagRelationRepository(DataContext dataContext) : INewsTagRelationRepository
{
    public async Task<List<NewsTagRelation>> GetListByExpression(
        Expression<Func<NewsTagRelation, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<NewsTagRelation, object>>[] includes)
    {
        var newsTagRelations = dataContext.NewsTagRelation.AsNoTracking();
        if (expression != null)
            newsTagRelations = newsTagRelations.Where(expression);
        Array.ForEach(includes, i => newsTagRelations = newsTagRelations.Include(i));
        return await newsTagRelations.ToListAsync(cancellationToken);
    }

    public async Task<NewsTagRelation?> GetByExpression(
        Expression<Func<NewsTagRelation, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<NewsTagRelation, object>>[] includes)
    {
        var newsTagRelations = dataContext.NewsTagRelation.AsNoTracking();
        Array.ForEach(includes, i => newsTagRelations = newsTagRelations.Include(i));
        return await newsTagRelations.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task CreateAsync(NewsTagRelation newsTagRelation, CancellationToken cancellationToken = default)
    {
        if (await dataContext.NewsTagRelation.AnyAsync(ntr => 
                    ntr.NewsId == newsTagRelation.NewsId
                    && ntr.NewsTagId == newsTagRelation.NewsTagId, 
                cancellationToken))
            throw new ArgumentException(string.Format(
                ErrorMessages.CoincideError,
                nameof(NewsTagRelation.NewsTagId) + "и" + nameof(NewsTagRelation.NewsId),
                nameof(NewsTagRelation)));
        await dataContext.NewsTagRelation.AddAsync(newsTagRelation, cancellationToken);
    }

    public async Task UpdateAsync(NewsTagRelation newsTagRelation, CancellationToken cancellationToken = default)
    {
        if (!await dataContext.NewsTagRelation.AnyAsync(ntr => ntr.Id == newsTagRelation.Id, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(NewsTagRelation)));
        dataContext.NewsTagRelation.Update(newsTagRelation);
    }

    public void Delete(NewsTagRelation newsTagRelation)
        => dataContext.NewsTagRelation.Remove(newsTagRelation);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);

    public async Task StartTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollBackTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.RollbackTransactionAsync(cancellationToken);
}