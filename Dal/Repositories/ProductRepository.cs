using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Dal.Repositories;

public class ProductRepository(DataContext dataContext) : IProductRepository
{
    public async Task CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        
    }

    public void Delete(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<Product?> GetByExpression(Expression<Func<Product, bool>> expression, CancellationToken cancellationToken = default, params Expression<Func<Product, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetListByExpression(Expression<Func<Product, bool>>? expression = null, CancellationToken cancellationToken = default, params Expression<Func<Product, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}