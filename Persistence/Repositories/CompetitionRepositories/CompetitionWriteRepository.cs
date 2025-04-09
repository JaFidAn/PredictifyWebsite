using Application.Repositories.CompetitionRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.CompetitionRepositories;

public class CompetitionWriteRepository : WriteRepository<Competition, int>, ICompetitionWriteRepository
{
    public CompetitionWriteRepository(ApplicationDbContext context) : base(context) { }
}
