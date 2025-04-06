using Application.Core;
using Application.DTOs.Competitions;
using Application.Params;

namespace Application.Services;

public interface ICompetitionService
{
    Task<Result<PagedResult<CompetitionDto>>> GetAllAsync(CompetitionFilterParams filters, CancellationToken cancellationToken);
    Task<Result<CompetitionDto>> GetByIdAsync(string id);
    Task<Result<CompetitionDto>> CreateAsync(CreateCompetitionDto dto);
    Task<Result<bool>> UpdateAsync(UpdateCompetitionDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}
