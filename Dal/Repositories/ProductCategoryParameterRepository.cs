using System.Data.Entity;
using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

public class ProductCategoryParameterRepository(DataContext dataContext) : IProductCategoryParameterRepository
{
    public async Task<List<ProductCategoryParameter>> GetListByExpression(
        Expression<Func<ProductCategoryParameter, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategoryParameter, object>>[] includes)
    {
        var productCategoryParameters = dataContext.ProductCategoryParameter.AsNoTracking();
        Array.ForEach(includes, i => productCategoryParameters = productCategoryParameters.Include(i));
        if (expression != null)
            productCategoryParameters = productCategoryParameters.Where(expression);
        return await productCategoryParameters.ToListAsync(cancellationToken);
    }

    public async Task<ProductCategoryParameter?> GetByExpression(
        Expression<Func<ProductCategoryParameter, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategoryParameter, object>>[] includes)
    {
        var productCategoryParameters = dataContext.ProductCategoryParameter.AsNoTracking();
        Array.ForEach(includes, i => productCategoryParameters = productCategoryParameters.Include(i));
        return await productCategoryParameters.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task CreateAsync(ProductCategoryParameter productCategoryParameter, CancellationToken cancellationToken = default)
    {
        if (await dataContext.ProductCategoryParameter.AnyAsync(pcp => 
                    pcp.Name == productCategoryParameter.Name,
                cancellationToken))
            throw new ArgumentException(string.Format(ErrorMessages.CoincideError, nameof(ProductCategoryParameter.Name), nameof(ProductCategoryParameter)));
        await dataContext.ProductCategoryParameter.AddAsync(productCategoryParameter, cancellationToken);
    }

    public async Task UpdateAsync(ProductCategoryParameter productCategoryParameter, CancellationToken cancellationToken = default)
    {
        if (!await dataContext.ProductCategoryParameter.AnyAsync(pcp => 
                    pcp.Id == productCategoryParameter.Id,
                cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError, nameof(ProductCategoryParameter)));
        dataContext.ProductCategoryParameter.Update(productCategoryParameter);
    }

    public void Delete(ProductCategoryParameter productCategoryParameter)
        => dataContext.ProductCategoryParameter.Remove(productCategoryParameter);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);

    public async Task StartTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollBackTransaction(CancellationToken cancellationToken = default)
        => await dataContext.Database.RollbackTransactionAsync(cancellationToken);
}