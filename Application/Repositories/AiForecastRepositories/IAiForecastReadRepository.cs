using Domain.Entities;

namespace Application.Repositories.AiForecastRepositories;

public interface IAiForecastReadRepository
{
    IQueryable<AiForecast> GetAll();
    Task<List<AiForecast>> GetByMatchIdAsync(int matchId);
}