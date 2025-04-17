using Application.Repositories.ForecastRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.ForecastRepositories;

public class ForecastWriteRepository : WriteRepository<Forecast, int>, IForecastWriteRepository
{
    private readonly ApplicationDbContext _context;

    public ForecastWriteRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task RemoveByMatchIdAsync(int matchId)
    {
        var toRemove = await _context.Forecasts
            .Where(f => f.MatchId == matchId)
            .ToListAsync();

        _context.Forecasts.RemoveRange(toRemove);
    }

    public async Task RemoveByTeamIdAsync(int teamId)
    {
        var toRemove = await _context.Forecasts
            .Where(f => f.TeamId == teamId)
            .ToListAsync();

        _context.Forecasts.RemoveRange(toRemove);
    }
    
    public async Task RemoveHardRangeAsync(List<Forecast> forecasts)
    {
        _context.Forecasts.RemoveRange(forecasts);
        await Task.CompletedTask;
    }
}