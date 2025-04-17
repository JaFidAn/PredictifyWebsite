using Application.Core;
using Application.DTOs.TeamOutcomeStreaks;
using Application.Params;

namespace Application.Services;

public interface ITeamOutcomeStreakService
{
    Task<Result<PagedResult<TeamOutcomeStreakDto>>> GetAllAsync(TeamOutcomeStreakFilterParams filters, CancellationToken cancellationToken);
    Task<Result<TeamOutcomeStreakDto>> GetByIdsAsync(int teamId, int outcomeId, int matchId);
    Task<Result<bool>> RecalculateStreaksForMatchAsync(int matchId);
    Task<Result<bool>> RecalculateAllAsync();
    Task<Result<bool>> RecalculateStreaksForTeamAsync(int teamId);
    Task<Result<bool>> RecalculateStreaksUpToMatchAsync(int teamId, int matchId);
    Task<Result<List<TeamOutcomeStreakDto>>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken);
    Task<Result<List<TeamOutcomeStreakDto>>> GetLatestByTeamIdAsync(int teamId, CancellationToken cancellationToken);
}