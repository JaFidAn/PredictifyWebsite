using Application.Repositories.AiForecastRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.AiForecastRepositories;

public class AiForecastReadRepository : IAiForecastReadRepository
{
    private readonly ApplicationDbContext _context;

    public AiForecastReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<AiForecast> GetAll()
    {
        return _context.AiForecasts.AsQueryable();
    }
}