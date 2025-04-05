using Application.Repositories.CountryRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.CountryRepositories;

public class CountryReadRepository : ReadRepository<Country>, ICountryReadRepository
{
    public CountryReadRepository(ApplicationDbContext context) : base(context) { }
}
