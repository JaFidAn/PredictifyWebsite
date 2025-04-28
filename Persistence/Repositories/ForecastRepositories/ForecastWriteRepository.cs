using Application.Repositories.ForecastRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.ForecastRepositories;

public class ForecastWriteRepository : IForecastWriteRepository
{
    private readonly ApplicationDbContext _context;

    public ForecastWriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(List<Forecast> forecasts)
    {
        await _context.Forecasts.AddRangeAsync(forecasts);
    }

    public async Task RemoveByMatchIdAsync(int matchId)
    {
        var forecasts = await _context.Forecasts
            .Where(f => f.MatchId == matchId)
            .ToListAsync();

        _context.Forecasts.RemoveRange(forecasts);
    }

    public async Task RemoveByTeamIdAsync(int teamId)
    {
        var forecasts = await _context.Forecasts
            .Where(f => f.TeamId == teamId)
            .ToListAsync();

        _context.Forecasts.RemoveRange(forecasts);
    }

    public Task RemoveHardRangeAsync(List<Forecast> forecasts)
    {
        _context.Forecasts.RemoveRange(forecasts);
        return Task.CompletedTask;
    }
    
    public async Task RemoveAllAsync()
    {
        var forecasts = await _context.Forecasts.ToListAsync();
        _context.Forecasts.RemoveRange(forecasts);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}