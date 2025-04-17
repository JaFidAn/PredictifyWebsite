using Application.Core;
using Application.DTOs.Forecasts;
using Application.Params;

namespace Application.Services;

public interface IForecastService
{
    Task<Result<PagedResult<ForecastDto>>> GetAllForecastsAsync(ForecastFilterParams filters, CancellationToken cancellationToken);
    Task<Result<bool>> GenerateForecastsForMatchAsync(int matchId);
    Task<Result<bool>> GenerateAllForecastsAsync();
    Task<Result<bool>> GenerateForecastsForMissingMatchesAsync();
    Task<Result<bool>> UpdateForecastsAfterMatchResultAsync(int matchId);
    Task<Result<List<ForecastDto>>> GetByMatchIdAsync(int matchId);
    Task<Result<List<ForecastSummaryDto>>> GetAllForecastSummariesAsync(CancellationToken cancellationToken);
    Task<Result<ForecastSummaryDto>> GetForecastSummaryByMatchIdAsync(int matchId);
}