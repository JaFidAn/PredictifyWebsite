using Application.Repositories.LeagueRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.LeagueRepositories;

public class LeagueReadRepository : ReadRepository<League, int>, ILeagueReadRepository
{
    public LeagueReadRepository(ApplicationDbContext context) : base(context) { }
}
