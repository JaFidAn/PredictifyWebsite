using Domain.Entities;

namespace Application.Repositories.TeamOutcomeStreakRepositories;

public interface ITeamOutcomeStreakReadRepository
{
    Task<List<TeamOutcomeStreak>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);
    IQueryable<TeamOutcomeStreak> GetAll();
}
