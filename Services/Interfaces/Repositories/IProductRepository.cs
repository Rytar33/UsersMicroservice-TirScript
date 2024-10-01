using Models;

namespace Services.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository<Product>, ITransactionRepository;