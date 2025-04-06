using Application.Repositories.LeagueRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.LeagueRepositories;

public class LeagueWriteRepository : WriteRepository<League>, ILeagueWriteRepository
{
    public LeagueWriteRepository(ApplicationDbContext context) : base(context) { }
}
