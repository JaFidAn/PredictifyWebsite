using Domain.Entities;

namespace Application.Repositories.TeamOutcomeStreakRepositories;

public interface ITeamOutcomeStreakWriteRepository : IWriteRepository<TeamOutcomeStreak, int>
{
    Task RemoveAllAsync();
    Task RemoveAllByTeamIdAsync(int teamId);
    Task RemoveByMatchIdAsync(int matchId);
    Task RemoveByTeamAndOutcomeAsync(int teamId, int outcomeId);
}
