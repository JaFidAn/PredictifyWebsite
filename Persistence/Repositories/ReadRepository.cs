using Application.Repositories;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using System.Linq.Expressions;

namespace Persistence.Repositories;

public class ReadRepository<T, TKey> : IReadRepository<T, TKey> where T : BaseEntity<TKey>
{
    private readonly ApplicationDbContext _context;

    public ReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> GetAll()
    {
        return _context.Set<T>().Where(x => !x.IsDeleted);
    }

    public async Task<List<T>> GetAllAsListAsync()
    {
        return await GetAll().ToListAsync();
    }

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> method)
    {
        return GetAll().Where(method);
    }

    public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> method)
    {
        return await GetWhere(method).FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(TKey id)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id!.Equals(id));
    }
}
