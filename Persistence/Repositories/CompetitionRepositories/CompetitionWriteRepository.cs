using Application.Repositories.CompetitionRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.CompetitionRepositories;

public class CompetitionWriteRepository : WriteRepository<Competition>, ICompetitionWriteRepository
{
    public CompetitionWriteRepository(ApplicationDbContext context) : base(context) { }
}
