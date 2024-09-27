using System.Data.Entity;
using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

public class NewsRepository(DataContext dataContext) : INewsRepository
{
    public async Task<List<News>> GetListByExpression(
        Expression<Func<News, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<News, object>>[] includes)
    {
        var news = dataContext.News.AsNoTracking();
        Array.ForEach(includes, i => news = news.Include(i));
        if (expression != null)
            news = news.Where(expression);
        return await news.ToListAsync(cancellationToken);
    }

    public async Task<News?> GetByExpression(
        Expression<Func<News, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<News, object>>[] includes)
    {
        var news = dataContext.News.AsNoTracking();
        Array.ForEach(includes, i => news = news.Include(i));
        return await news.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task CreateAsync(News news, CancellationToken cancellationToken = default)
    {
        if (await dataContext.News.AnyAsync(n => n.Title == news.Title, cancellationToken))
            throw new ArgumentException(string.Format(ErrorMessages.CoincideError, nameof(News.Title), nameof(News)));
        await dataContext.News.AddAsync(news, cancellationToken);
    }

    public async Task UpdateAsync(News news, CancellationToken cancellationToken = default)
    {
        if (!await dataContext.News.AnyAsync(n => n.Id == news.Id, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(News)));
        dataContext.News.Update(news);
    }

    public void Delete(News news)
        => dataContext.News.Remove(news);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);
}