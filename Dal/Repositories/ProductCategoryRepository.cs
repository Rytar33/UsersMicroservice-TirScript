using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Dal.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    public Task CreateAsync(ProductCategory entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Delete(ProductCategory entity)
    {
        throw new NotImplementedException();
    }

    public Task<ProductCategory?> GetByExpression(Expression<Func<ProductCategory, bool>> expression, CancellationToken cancellationToken = default, params Expression<Func<ProductCategory, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductCategory>> GetListByExpression(Expression<Func<ProductCategory, bool>>? expression = null, CancellationToken cancellationToken = default, params Expression<Func<ProductCategory, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ProductCategory entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}