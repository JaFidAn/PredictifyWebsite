using Application.Repositories.TeamOutcomeStreakRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.TeamOutcomeStreakRepositories;

public class TeamOutcomeStreakReadRepository : ITeamOutcomeStreakReadRepository
{
    private readonly ApplicationDbContext _context;

    public TeamOutcomeStreakReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<TeamOutcomeStreak> GetAll()
    {
        return _context.TeamOutcomeStreaks.AsQueryable();
    }

    public async Task<List<TeamOutcomeStreak>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return await _context.TeamOutcomeStreaks
            .Where(x => x.TeamId == teamId)
            .Include(x => x.Team)
            .Include(x => x.Outcome)
            .OrderByDescending(x => x.MatchDate)
            .ToListAsync(cancellationToken);
    }
}