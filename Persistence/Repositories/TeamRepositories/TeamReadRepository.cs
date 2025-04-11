using Application.Repositories.TeamRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.TeamRepositories;

public class TeamReadRepository : ReadRepository<Team, int>, ITeamReadRepository
{
    public TeamReadRepository(ApplicationDbContext context) : base(context) { }
}
