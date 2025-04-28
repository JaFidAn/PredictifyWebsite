using Domain.Entities;

namespace Application.Repositories.ForecastRepositories;

public interface IForecastReadRepository
{
    Task<List<Forecast>> GetByMatchIdAsync(int matchId, CancellationToken cancellationToken = default);
    Task<List<Forecast>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);
    IQueryable<Forecast> GetAll();
    IQueryable<Forecast> GetAllWithIncludes();
}