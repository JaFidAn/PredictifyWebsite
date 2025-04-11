using Application.Core;
using Application.DTOs.Outcomes;
using Application.Params;

namespace Application.Services;

public interface IOutcomeService
{
    Task<Result<PagedResult<OutcomeDto>>> GetAllAsync(OutcomeFilterParams filters, CancellationToken cancellationToken);
    Task<Result<OutcomeDto>> GetByIdAsync(int id);
    Task<Result<OutcomeDto>> CreateAsync(CreateOutcomeDto dto);
    Task<Result<bool>> UpdateAsync(UpdateOutcomeDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}
