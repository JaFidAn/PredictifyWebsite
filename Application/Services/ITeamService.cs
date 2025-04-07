using Application.Core;
using Application.DTOs.Teams;
using Application.Params;

namespace Application.Services;

public interface ITeamService
{
    Task<Result<PagedResult<TeamDto>>> GetAllAsync(TeamFilterParams filters, CancellationToken cancellationToken);
    Task<Result<TeamDto>> GetByIdAsync(string id);
    Task<Result<TeamDto>> CreateAsync(CreateTeamDto dto);
    Task<Result<bool>> UpdateAsync(UpdateTeamDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}