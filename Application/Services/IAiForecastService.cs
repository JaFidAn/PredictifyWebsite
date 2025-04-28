using Application.Core;
using Application.DTOs.AiForecasts;
using Application.Params;

namespace Application.Services;

public interface IAiForecastService
{
    Task<Result<AiForecastDto>> GetByForecastIdAsync(int forecastId);
    Task<Result<List<AiForecastDto>>> GetAllByMatchIdAsync(int matchId);
    Task<Result<List<AiForecastDto>>> GetAllByMatchIdWithTeamsAsync(int matchId);
    Task<Result<bool>> AddAiForecastAsync(int forecastId, string predictedOutcomeName, double confidence, string modelVersion);
    Task<Result<bool>> SetIsCorrectAsync(int forecastId, bool isCorrect);
    Task<Result<double>> GetAccuracyAsync(AiForecastFilterParams filters);
    Task<Result<PagedResult<AiForecastTrendDto>>> GetTrendReportAsync(AiForecastFilterParams filters);
    Task<Result<PagedResult<ForecastComparisonDto>>> GetForecastComparisonAsync(AiForecastFilterParams filters);
    Task<AiForecastPreviewDto> PreviewForecastAsync(int matchId);
    Task<Result<bool>> GenerateAiForecastForLastSeasonAsync(int seasonId);
    Task<Result<bool>> RemoveByForecastIdsAsync(List<int> forecastIds);
    Task<Result<bool>> RemoveAllAsync();
}