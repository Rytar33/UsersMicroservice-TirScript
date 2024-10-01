using System.Data.Entity;
using Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Models.Exceptions;
using Models.Validations;

namespace Dal.Repositories;

public class ProductCategoryParameterValueProductRepository(DataContext dataContext) : IProductCategoryParameterValueProductRepository
{
    public async Task<List<ProductCategoryParameterValueProduct>> GetListByExpression(
        Expression<Func<ProductCategoryParameterValueProduct, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategoryParameterValueProduct, object>>[] includes)
    {
        var productCategoryParameterValueProducts = QueryableExtensions.AsNoTracking(dataContext.ProductCategoryParameterValueProduct);
        Array.ForEach(includes, i => productCategoryParameterValueProducts = QueryableExtensions.Include(productCategoryParameterValueProducts, i));
        if (expression != null)
            productCategoryParameterValueProducts = productCategoryParameterValueProducts.Where(expression);
        return await QueryableExtensions.ToListAsync(productCategoryParameterValueProducts, cancellationToken);
    }

    public async Task<ProductCategoryParameterValueProduct?> GetByExpression(
        Expression<Func<ProductCategoryParameterValueProduct, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategoryParameterValueProduct, object>>[] includes)
    {
        var productCategoryParameterValueProducts = QueryableExtensions.AsNoTracking(dataContext.ProductCategoryParameterValueProduct);
        Array.ForEach(includes, i => productCategoryParameterValueProducts = QueryableExtensions.Include(productCategoryParameterValueProducts, i));
        return await QueryableExtensions.FirstOrDefaultAsync(productCategoryParameterValueProducts, expression, cancellationToken);
    }

    public async Task CreateAsync(ProductCategoryParameterValueProduct productCategoryParameterValueProduct, CancellationToken cancellationToken = default)
    {
        if (await QueryableExtensions.AnyAsync(dataContext.ProductCategoryParameterValueProduct, pcpvp =>
                pcpvp.ProductId == productCategoryParameterValueProduct.ProductId
                && pcpvp.ProductCategoryParameterValueId ==
                productCategoryParameterValueProduct.ProductCategoryParameterValueId,
                cancellationToken))
            throw new ArgumentException(string.Format(
                ErrorMessages.CoincideError,
                nameof(ProductCategoryParameterValueProduct.Id),
                nameof(ProductCategoryParameterValueProduct)));
        await dataContext.ProductCategoryParameterValueProduct.AddAsync(productCategoryParameterValueProduct, cancellationToken);
    }

    public async Task UpdateAsync(ProductCategoryParameterValueProduct productCategoryParameterValueProduct, CancellationToken cancellationToken = default)
    {
        if (!await QueryableExtensions.AnyAsync(dataContext.ProductCategoryParameterValueProduct, pcpvp => 
                pcpvp.Id == productCategoryParameterValueProduct.Id, cancellationToken))
            throw new NotFoundException(string.Format(ErrorMessages.NotFoundError,
                nameof(ProductCategoryParameterValueProduct)));
        dataContext.ProductCategoryParameterValueProduct.Update(productCategoryParameterValueProduct);
        await dataContext.ProductCategoryParameterValueProduct.Where(p => p.ProductId == 1).ExecuteDeleteAsync(cancellationToken);
    }

    public void Delete(ProductCategoryParameterValueProduct productCategoryParameterValueProduct)
        => dataContext.ProductCategoryParameterValueProduct.Remove(productCategoryParameterValueProduct);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dataContext.SaveChangesAsync(cancellationToken);
}