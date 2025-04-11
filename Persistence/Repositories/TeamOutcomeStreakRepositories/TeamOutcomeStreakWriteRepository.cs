using Application.Repositories.TeamOutcomeStreakRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.TeamOutcomeStreakRepositories;

public class TeamOutcomeStreakWriteRepository : WriteRepository<TeamOutcomeStreak, int>, ITeamOutcomeStreakWriteRepository
{
    private readonly ApplicationDbContext _context;

    public TeamOutcomeStreakWriteRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task RemoveAllAsync()
    {
        var all = await _context.TeamOutcomeStreaks.ToListAsync();
        _context.TeamOutcomeStreaks.RemoveRange(all);
    }

    public async Task RemoveAllByTeamIdAsync(int teamId)
    {
        var toRemove = await _context.TeamOutcomeStreaks
            .Where(x => x.TeamId == teamId)
            .ToListAsync();

        _context.TeamOutcomeStreaks.RemoveRange(toRemove);
    }

    public async Task RemoveByMatchIdAsync(int matchId)
    {
        var toRemove = await _context.TeamOutcomeStreaks
            .Where(x => x.MatchId == matchId)
            .ToListAsync();

        _context.TeamOutcomeStreaks.RemoveRange(toRemove);
    }

    public async Task RemoveByTeamAndOutcomeAsync(int teamId, int outcomeId)
    {
        var toRemove = await _context.TeamOutcomeStreaks
            .Where(x => x.TeamId == teamId && x.OutcomeId == outcomeId)
            .ToListAsync();

        _context.TeamOutcomeStreaks.RemoveRange(toRemove);
    }
}
