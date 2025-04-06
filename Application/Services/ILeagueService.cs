using Application.Core;
using Application.DTOs.Leagues;
using Application.Params;

namespace Application.Services;

public interface ILeagueService
{
    Task<Result<PagedResult<LeagueDto>>> GetAllAsync(LeagueFilterParams filters, CancellationToken cancellationToken);
    Task<Result<LeagueDto>> GetByIdAsync(string id);
    Task<Result<LeagueDto>> CreateAsync(CreateLeagueDto dto);
    Task<Result<bool>> UpdateAsync(UpdateLeagueDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}
