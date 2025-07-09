using Application.Repositories.AiForecastRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<AiForecast>> GetByMatchIdAsync(int matchId) 
    {
        return await _context.AiForecasts
            .Where(x => x.MatchId == matchId)
            .ToListAsync();
    }
}