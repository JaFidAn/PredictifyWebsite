using Application.Repositories.AiForecastRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.AiForecastRepositories;

public class AiForecastWriteRepository : IAiForecastWriteRepository
{
    private readonly ApplicationDbContext _context;

    public AiForecastWriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AiForecast entity)
    {
        await _context.AiForecasts.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAllAsync()
    {
        var aiForecasts = await _context.AiForecasts.ToListAsync();
        _context.AiForecasts.RemoveRange(aiForecasts);
    }

    public async Task RemoveByForecastIdsAsync(List<int> forecastIds)
    {
        var aiForecasts = await _context.AiForecasts
            .Where(x => forecastIds.Contains(x.ForecastId))
            .ToListAsync();

        if (aiForecasts.Any())
        {
            _context.AiForecasts.RemoveRange(aiForecasts);
        }
    }
}