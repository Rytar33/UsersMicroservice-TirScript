namespace Services.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория транзакций
/// </summary>
public interface ITransactionRepository
{
    Task StartTransaction(CancellationToken cancellationToken = default);

    Task CommitTransaction(CancellationToken cancellationToken = default);

    Task RollBackTransaction(CancellationToken cancellationToken = default);
}