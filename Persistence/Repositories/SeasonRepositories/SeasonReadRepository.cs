using Application.Repositories.SeasonRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.SeasonRepositories;

public class SeasonReadRepository : ReadRepository<Season>, ISeasonReadRepository
{
    public SeasonReadRepository(ApplicationDbContext context) : base(context) { }
}
