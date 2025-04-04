using Application.Repositories;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;

    public WriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(T model)
    {
        await _context.Set<T>().AddAsync(model);
        return true;
    }

    public async Task<bool> AddRangeAsync(List<T> models)
    {
        await _context.Set<T>().AddRangeAsync(models);
        return true;
    }

    public bool Remove(T model)
    {
        model.IsDeleted = true;
        _context.Set<T>().Update(model);
        return true;
    }

    public bool RemoveRange(List<T> models)
    {
        foreach (var model in models)
        {
            model.IsDeleted = true;
        }

        _context.Set<T>().UpdateRange(models);
        return true;
    }

    public async Task<bool> RemoveAsync(string id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
        {
            return false;
        }

        entity.IsDeleted = true;
        _context.Set<T>().Update(entity);
        return true;
    }

    public bool Update(T model)
    {
        _context.Set<T>().Update(model);
        return true;
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    private async Task<T?> GetByIdAsync(string id)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }
}
