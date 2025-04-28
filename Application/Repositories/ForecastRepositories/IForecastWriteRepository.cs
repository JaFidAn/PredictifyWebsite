using Domain.Entities;

namespace Application.Repositories.ForecastRepositories;

public interface IForecastWriteRepository
{
    Task AddRangeAsync(List<Forecast> forecasts);
    Task RemoveByMatchIdAsync(int matchId);
    Task RemoveByTeamIdAsync(int teamId);
    Task RemoveHardRangeAsync(List<Forecast> forecasts);
    Task RemoveAllAsync();
    Task SaveChangesAsync();
}