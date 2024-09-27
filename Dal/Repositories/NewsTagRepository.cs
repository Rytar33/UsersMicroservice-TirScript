using System.Data.Entity;
using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

public class NewsTagRepository(DataContext dataContext) : INewsTagRepository
{
    public async Task<List<NewsTag>> GetListByExpression(
        Expression<Func<NewsTag, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<NewsTag, object>>[] includes)
    {
        var newsTags = dataContext.NewsTag.AsNoTracking();
        Array.ForEach(includes, i => newsTags = newsTags.Include(i));
        if (expression != null)
            newsTags = newsTags.Where(expression);
        return await newsTags.ToListAsync(cancellationToken);
    }

    public async Task<NewsTag?> GetByExpression(
        Expression<Func<NewsTag, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<NewsTag, object>>[] includes)
    {
        var newsTags = dataContext.NewsTag.AsNoTracking();
        Array.ForEach(includes, i => newsTags = newsTags.Include(i));
        return await newsTags.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task CreateAsync(NewsTag newsTag, CancellationToken cancellationToken = default)
    {
        if (await dataContext.NewsTag.AnyAsync(nt => nt.Name == newsTag.Name, cancellationToken))
            throw new ArgumentException(string.Format(ErrorMessages.CoincideError, nameof(NewsTag.Name), nameof(NewsTag)));
        await dataContext.NewsTag.AddAsync(newsTag, cancellationToken);
    }

    public async Task UpdateAsync(NewsTag newsTag, CancellationToken cancellationToken = default)
    {
        if (!await dataContext.NewsTag.AnyAsync(nt => nt.Id == newsTag.Id, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(NewsTag)));
        dataContext.NewsTag.Update(newsTag);
    }

    public void Delete(NewsTag newsTag)
        => dataContext.NewsTag.Remove(newsTag);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);
}