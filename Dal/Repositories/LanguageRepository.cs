using System.Data.Entity;
using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

public class LanguageRepository(DataContext dataContext) : ILanguageRepository
{
    public async Task<List<Language>> GetListByExpression(
        Expression<Func<Language, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<Language, object>>[] includes)
    {
        var languages = dataContext.Language.AsNoTracking();
        if (expression != null)
            languages = languages.Where(expression);
        Array.ForEach(includes, i => languages = languages.Include(i));
        return await languages.ToListAsync(cancellationToken);
    }

    public async Task<Language?> GetByExpression(
        Expression<Func<Language, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<Language, object>>[] includes)
    {
        var languages = dataContext.Language.AsNoTracking();
        Array.ForEach(includes, i => languages = languages.Include(i));
        return await languages.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task CreateAsync(Language language, CancellationToken cancellationToken = default)
    {
        if (await dataContext.Language.AnyAsync(l => l.Name == language.Name, cancellationToken))
            throw new ArgumentException(string.Format(ErrorMessages.CoincideError, nameof(Language.Name)));
        if (await dataContext.Language.AnyAsync(l => l.Code == language.Code, cancellationToken))
            throw new ArgumentException(string.Format(ErrorMessages.CoincideError, nameof(Language.Code)));
        await dataContext.Language.AddAsync(language, cancellationToken);
    }

    public async Task UpdateAsync(Language language, CancellationToken cancellationToken = default)
    {
        if (!await dataContext.Language.AnyAsync(l => l.Id == language.Id, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(Language)));
        dataContext.Language.Update(language);
    }

    public void Delete(Language language)
        => dataContext.Language.Remove(language);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);
}