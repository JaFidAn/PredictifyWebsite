using Application.Repositories.LeagueRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.LeagueRepositories;

public class LeagueWriteRepository : WriteRepository<League, int>, ILeagueWriteRepository
{
    public LeagueWriteRepository(ApplicationDbContext context) : base(context) { }
}
