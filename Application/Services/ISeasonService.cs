using Application.Core;
using Application.DTOs.Seasons;
using Application.Params;

namespace Application.Services;

public interface ISeasonService
{
    Task<Result<PagedResult<SeasonDto>>> GetAllAsync(SeasonFilterParams filters, CancellationToken cancellationToken);
    Task<Result<SeasonDto>> GetByIdAsync(int id);
    Task<Result<SeasonDto>> CreateAsync(CreateSeasonDto dto);
    Task<Result<bool>> UpdateAsync(UpdateSeasonDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}
