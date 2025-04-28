using Application.Repositories.ForecastRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.ForecastRepositories;

public class ForecastReadRepository : IForecastReadRepository
{
    private readonly ApplicationDbContext _context;

    public ForecastReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Forecast>> GetByMatchIdAsync(int matchId, CancellationToken cancellationToken = default)
    {
        return await _context.Forecasts
            .Where(f => f.MatchId == matchId)
            .Include(f => f.Team)
            .Include(f => f.Outcome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Forecast>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Forecasts
            .Where(f => f.TeamId == teamId)
            .Include(f => f.Match)
            .Include(f => f.Outcome)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<Forecast> GetAll()
    {
        return _context.Forecasts.AsQueryable();
    }

    public IQueryable<Forecast> GetAllWithIncludes()
    {
        return _context.Forecasts
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .Include(x => x.Match)
            .ThenInclude(m => m.MatchTeamSeasonLeagues);
    }
}