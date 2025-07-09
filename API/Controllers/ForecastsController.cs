using Application.Core;
using Application.DTOs.Forecasts;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ForecastsController : BaseApiController
{
    private readonly IForecastService _forecastService;
    private readonly IForecastAiService _forecastAiService;

    public ForecastsController(
        IForecastService forecastService,
        IForecastAiService forecastAiService)
    {
        _forecastService = forecastService;
        _forecastAiService = forecastAiService;
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
    public async Task<IActionResult> GetAllSummaries([FromQuery] ForecastFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _forecastService.GetAllForecastSummariesAsync(filters, cancellationToken);
        return HandleResult(result);
    }
    
    /// <summary>
    /// Get Classic forecast accuracy (% of correct predictions) with filters
    /// </summary>
    [HttpGet("accuracy")]
    public async Task<IActionResult> GetClassicForecastAccuracy([FromQuery] ForecastFilterParams filters)
    {
        var result = await _forecastService.GetClassicForecastAccuracyAsync(filters);
        if (!result.IsSuccess)
            return Problem(result.Error);

        return Ok(new { accuracy = result.Value });
    }
    
    /// <summary>
    /// Get Classic forecast trend (daily accuracy stats) with filtering
    /// </summary>
    [HttpGet("trend")]
    [ProducesResponseType(typeof(PagedResult<ForecastTrendDto>), 200)]
    public async Task<IActionResult> GetTrend([FromQuery] ForecastFilterParams filters)
    {
        var result = await _forecastService.GetTrendReportAsync(filters);
        if (!result.IsSuccess)
            return Problem(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Predict best forecast outcome for a specific match using AI
    /// </summary>
    [HttpGet("predict-ai/{matchId}")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> PredictByAi(int matchId)
    {
        var result = await _forecastAiService.PredictBestOutcomeAsync(matchId);
        return Ok(result);
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
    /// Generate forecasts for missing matches only
    /// </summary>
    [HttpPost("generate-missing")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<IActionResult> GenerateForMissingMatches()
    {
        var result = await _forecastService.GenerateForecastsForMissingMatchesAsync();
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
}
