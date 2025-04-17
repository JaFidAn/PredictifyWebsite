using Domain.Entities;

namespace Application.Repositories.TeamOutcomeStreakRepositories;

public interface ITeamOutcomeStreakWriteRepository
{
    Task AddAsync(TeamOutcomeStreak entity);
    Task RemoveRangeByTeamAndMatchDateAsync(int teamId, DateTime maxDate);
}
