using Application.Repositories.OutcomeRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.OutcomeRepositories;

public class OutcomeWriteRepository : WriteRepository<Outcome>, IOutcomeWriteRepository
{
    public OutcomeWriteRepository(ApplicationDbContext context) : base(context) { }
}
