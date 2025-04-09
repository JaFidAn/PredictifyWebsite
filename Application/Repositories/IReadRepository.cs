using System.Linq.Expressions;
using Domain.Entities.Common;

namespace Application.Repositories;

public interface IReadRepository<T, TKey> : IRepository<T, TKey> where T : BaseEntity<TKey>
{
    IQueryable<T> GetAll();
    Task<List<T>> GetAllAsListAsync();
    IQueryable<T> GetWhere(Expression<Func<T, bool>> method);
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> method);
    Task<T?> GetByIdAsync(TKey id);
}

