using Application.Repositories.CompetitionRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.CompetitionRepositories;

public class CompetitionReadRepository : ReadRepository<Competition, int>, ICompetitionReadRepository
{
    public CompetitionReadRepository(ApplicationDbContext context) : base(context) { }
}
