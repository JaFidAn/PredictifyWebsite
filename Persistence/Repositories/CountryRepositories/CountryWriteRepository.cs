using Application.Repositories.CountryRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.CountryRepositories;

public class CountryWriteRepository : WriteRepository<Country, int>, ICountryWriteRepository
{
    public CountryWriteRepository(ApplicationDbContext context) : base(context) { }
}
