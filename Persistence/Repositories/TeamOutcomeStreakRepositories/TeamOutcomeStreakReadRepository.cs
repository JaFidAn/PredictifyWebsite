using Application.Repositories.TeamOutcomeStreakRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.TeamOutcomeStreakRepositories;

public class TeamOutcomeStreakReadRepository : ReadRepository<TeamOutcomeStreak, int>, ITeamOutcomeStreakReadRepository
{
    public TeamOutcomeStreakReadRepository(ApplicationDbContext context) : base(context)
    {
    }
}
