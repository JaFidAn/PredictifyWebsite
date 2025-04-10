using Application.Repositories.MatchRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.MatchRepositories;

public class MatchReadRepository : ReadRepository<Match, int>, IMatchReadRepository
{
    public MatchReadRepository(ApplicationDbContext context) : base(context) { }
}
