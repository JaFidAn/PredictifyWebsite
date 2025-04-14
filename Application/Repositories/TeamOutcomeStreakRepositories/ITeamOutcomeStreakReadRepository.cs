using Domain.Entities;

namespace Application.Repositories.TeamOutcomeStreakRepositories;

public interface ITeamOutcomeStreakReadRepository : IReadRepository<TeamOutcomeStreak, int>
{
    Task<List<TeamOutcomeStreak>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);
}
