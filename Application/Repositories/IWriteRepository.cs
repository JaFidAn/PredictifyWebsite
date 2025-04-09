using Domain.Entities.Common;

namespace Application.Repositories;

public interface IWriteRepository<T, TKey> : IRepository<T, TKey> where T : BaseEntity<TKey>
{
    Task<bool> AddAsync(T model);
    Task<bool> AddRangeAsync(List<T> models);
    bool Remove(T model);
    bool RemoveRange(List<T> models);
    Task<bool> RemoveAsync(TKey id);
    bool Update(T model);
    Task<int> SaveAsync();
}

