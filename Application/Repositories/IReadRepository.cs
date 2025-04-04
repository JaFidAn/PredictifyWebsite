using System.Linq.Expressions;
using Domain.Entities.Common;

namespace Application.Repositories;

public interface IReadRepository<T> : IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll();
    Task<List<T>> GetAllAsListAsync();
    IQueryable<T> GetWhere(Expression<Func<T, bool>> method);
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> method);
    Task<T?> GetByIdAsync(string id);
}
