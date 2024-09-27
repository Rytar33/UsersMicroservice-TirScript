using Models;

namespace Services.Interfaces.Repositories;

public interface INewsTagRelationRepository : IBaseRepository<NewsTagRelation>, ITransactionRepository;