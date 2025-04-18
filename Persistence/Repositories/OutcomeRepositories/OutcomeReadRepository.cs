using Application.Repositories.OutcomeRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.OutcomeRepositories;

public class OutcomeReadRepository : ReadRepository<Outcome, int>, IOutcomeReadRepository
{
    public OutcomeReadRepository(ApplicationDbContext context) : base(context) { }
}
