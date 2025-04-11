using Application.Core;
using Application.DTOs.Matches;
using Application.Params;

namespace Application.Services;

public interface IMatchService
{
    Task<Result<PagedResult<MatchDto>>> GetAllAsync(MatchFilterParams filters, CancellationToken cancellationToken);
    Task<Result<MatchDto>> GetByIdAsync(int id);
    Task<Result<MatchDto>> CreateAsync(CreateMatchDto dto);
    Task<Result<bool>> UpdateAsync(UpdateMatchDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}
