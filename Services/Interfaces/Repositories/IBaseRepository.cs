using System.Linq.Expressions;
using Models;

namespace Services.Interfaces.Repositories;

/// <summary>
/// Базовый интерфейс под репозитории
/// </summary>
/// <typeparam name="TEntity">Сущность</typeparam>
public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Получение сущности по лямбда выражению ассинхронно
    /// </summary>
    /// <param name="expression">Лямбда выражение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Если находит такую сущность, возвращает её, иначе возвращает null</returns>
    Task<TEntity?> GetByExpression(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Получение списка сущностей по лямбда выражению ассинхронно
    /// </summary>
    /// <param name="expression">Лямбда выражение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Возвращает список сущностей</returns>
    Task<List<TEntity>> GetListByExpression(
        Expression<Func<TEntity, bool>>? expression = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Создание сущности ассинхронно
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление сущности ассинхронно
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление сущности
    /// </summary>
    /// <param name="entity">Сущность</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Сохранение изменений ассинхронно
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}