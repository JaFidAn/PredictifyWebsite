using Domain.Entities;

namespace Application.Repositories.CountryRepositories;

public interface ICountryReadRepository : IReadRepository<Country, int>
{
}
