using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Dal.Repositories;

public class ProductCategoryParameterValueRepository : IProductCategoryParameterValueRepository
{
    public Task<List<ProductCategoryParameterValue>> GetListByExpression(Expression<Func<ProductCategoryParameterValue, bool>>? expression = null, CancellationToken cancellationToken = default, params Expression<Func<ProductCategoryParameterValue, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    public Task<ProductCategoryParameterValue?> GetByExpression(Expression<Func<ProductCategoryParameterValue, bool>> expression, CancellationToken cancellationToken = default, params Expression<Func<ProductCategoryParameterValue, object>>[] includes)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(ProductCategoryParameterValue entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ProductCategoryParameterValue entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Delete(ProductCategoryParameterValue entity)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}