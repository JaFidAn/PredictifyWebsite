using Application.Core;
using Application.DTOs.Countries;
using Application.Params;

namespace Application.Services;

public interface ICountryService
{
    Task<Result<PagedResult<CountryDto>>> GetAllAsync(CountryFilterParams filters, CancellationToken cancellationToken);
    Task<Result<CountryDto>> GetByIdAsync(string id);
    Task<Result<CountryDto>> CreateAsync(CreateCountryDto dto);
    Task<Result<bool>> UpdateAsync(UpdateCountryDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}
