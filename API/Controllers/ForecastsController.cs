using Application.DTOs.Forecasts;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ForecastsController : BaseApiController
{
    private readonly IForecastService _forecastService;

    public ForecastsController(IForecastService forecastService)
    {
        _forecastService = forecastService;
    }
    
    /// <summary>
    /// Get forecast summary for a specific match
    /// </summary>
    [HttpGet("summary/{matchId}")]
    [ProducesResponseType(typeof(ForecastSummaryDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetSummaryByMatchId(int matchId)
    {
        var result = await _forecastService.GetForecastSummaryByMatchIdAsync(matchId);
        return HandleResult(result);
    }

    /// <summary>
    /// Get forecast summaries for all matches that are not completed yet
    /// </summary>
    [HttpGet("summaries")]
    [ProducesResponseType(typeof(List<ForecastSummaryDto>), 200)]
    public async Task<IActionResult> GetAllSummaries(CancellationToken cancellationToken)
    {
        var result = await _forecastService.GetAllForecastSummariesAsync(cancellationToken);
        return HandleResult(result);
    }


    /// <summary>
    /// Generate forecasts for a specific match
    /// </summary>
    [HttpPost("generate/{matchId}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GenerateForMatch(int matchId)
    {
        var result = await _forecastService.GenerateForecastsForMatchAsync(matchId);
        return HandleResult(result);
    }

    /// <summary>
    /// Generate forecasts for all matches
    /// </summary>
    [HttpPost("generate-all")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<IActionResult> GenerateAll()
    {
        var result = await _forecastService.GenerateAllForecastsAsync();
        return HandleResult(result);
    }

    /// <summary>
    /// Update forecast IsCorrect values after a match is completed
    /// </summary>
    [HttpPut("update-after-result/{matchId}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> UpdateAfterResult(int matchId)
    {
        var result = await _forecastService.UpdateForecastsAfterMatchResultAsync(matchId);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all forecasts for a specific match
    /// </summary>
    [HttpGet("by-match/{matchId}")]
    [ProducesResponseType(typeof(List<ForecastDto>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetByMatch(int matchId)
    {
        var result = await _forecastService.GetByMatchIdAsync(matchId);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all forecasts with optional IsCorrect filter (true, false, or null)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ForecastDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] ForecastFilterParams filter, CancellationToken cancellationToken)
    {
        var result = await _forecastService.GetAllForecastsAsync(filter, cancellationToken);
        return HandleResult(result);
    }
    
    [HttpPost("generate-missing")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<IActionResult> GenerateForMissingMatches()
    {
        var result = await _forecastService.GenerateForecastsForMissingMatchesAsync();
        return HandleResult(result);
    }

}
