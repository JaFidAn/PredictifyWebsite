using Application.Repositories.TeamOutcomeStreakRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.TeamOutcomeStreakRepositories;

public class TeamOutcomeStreakWriteRepository : ITeamOutcomeStreakWriteRepository
{
    private readonly ApplicationDbContext _context;

    public TeamOutcomeStreakWriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TeamOutcomeStreak entity)
    {
        await _context.TeamOutcomeStreaks.AddAsync(entity);
    }

    public async Task RemoveRangeByTeamAndMatchDateAsync(int teamId, DateTime maxDate)
    {
        var toRemove = await _context.TeamOutcomeStreaks
            .Where(x => x.TeamId == teamId && x.MatchDate <= maxDate)
            .ToListAsync();

        _context.TeamOutcomeStreaks.RemoveRange(toRemove);
    }
}