using Application.Core;
using Application.DTOs.Countries;

namespace Application.Services;

public interface ICountryService
{
    Task<Result<PagedResult<CountryDto>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken);
    Task<Result<CountryDto>> GetByIdAsync(string id);
    Task<Result<CountryDto>> CreateAsync(CreateCountryDto dto);
    Task<Result<bool>> UpdateAsync(UpdateCountryDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}
