using Domain.Entities;

namespace Application.Repositories.AiForecastRepositories;

public interface IAiForecastWriteRepository
{
    Task AddAsync(AiForecast entity);
    Task RemoveAllAsync();
    Task RemoveByForecastIdsAsync(List<int> forecastIds);
    Task SaveChangesAsync();
}