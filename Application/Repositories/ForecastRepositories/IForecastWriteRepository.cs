using Domain.Entities;

namespace Application.Repositories.ForecastRepositories;

public interface IForecastWriteRepository : IWriteRepository<Forecast, int>
{
    Task RemoveByMatchIdAsync(int matchId);
    Task RemoveByTeamIdAsync(int teamId);
    Task RemoveHardRangeAsync(List<Forecast> forecasts);
}