using Models;

namespace Services.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория контактов пользователя
/// </summary>
public interface IUserContactRepository : IBaseRepository<UserContact>, ITransactionRepository;