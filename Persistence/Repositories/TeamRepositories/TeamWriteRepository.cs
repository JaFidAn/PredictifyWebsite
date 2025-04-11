using Application.Repositories.TeamRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.TeamRepositories;

public class TeamWriteRepository : WriteRepository<Team, int>, ITeamWriteRepository
{
    private readonly ApplicationDbContext _context;

    public TeamWriteRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void RemoveTeamSeasonLeagues(Team team)
    {
        var leagues = team.TeamSeasonLeagues.ToList();
        _context.TeamSeasonLeagues.RemoveRange(leagues);
    }
}
