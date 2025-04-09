using Application.Repositories.SeasonRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.SeasonRepositories;

public class SeasonWriteRepository : WriteRepository<Season, int>, ISeasonWriteRepository
{
    public SeasonWriteRepository(ApplicationDbContext context) : base(context) { }
}
