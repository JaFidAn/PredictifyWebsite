using Application.Repositories.ForecastRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.ForecastRepositories;

public class ForecastReadRepository : ReadRepository<Forecast, int>, IForecastReadRepository
{
    private readonly ApplicationDbContext _context;

    public ForecastReadRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Forecast>> GetByMatchIdAsync(int matchId, CancellationToken cancellationToken = default)
    {
        return await _context.Forecasts
            .Where(f => f.MatchId == matchId && !f.IsDeleted)
            .Include(f => f.Team)
            .Include(f => f.Outcome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Forecast>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Forecasts
            .Where(f => f.TeamId == teamId && !f.IsDeleted)
            .Include(f => f.Match)
            .Include(f => f.Outcome)
            .ToListAsync(cancellationToken);
    }
}