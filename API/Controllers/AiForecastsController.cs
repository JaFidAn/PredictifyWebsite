using Application.Core;
using Application.DTOs.AiForecasts;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AiForecastsController : BaseApiController
{
    private readonly IAiForecastService _aiForecastService;

    public AiForecastsController(IAiForecastService aiForecastService)
    {
        _aiForecastService = aiForecastService;
    }

    /// <summary>
    /// Get AI forecast by ForecastId (links to classic forecast)
    /// </summary>
    [HttpGet("by-forecast/{forecastId}")]
    [ProducesResponseType(typeof(AiForecastDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetByForecastId(int forecastId)
    {
        var result = await _aiForecastService.GetByForecastIdAsync(forecastId);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all AI forecasts for a specific match
    /// </summary>
    [HttpGet("by-match/{matchId}")]
    [ProducesResponseType(typeof(List<AiForecastDto>), 200)]
    public async Task<IActionResult> GetByMatchId(int matchId)
    {
        var result = await _aiForecastService.GetAllByMatchIdWithTeamsAsync(matchId);
        return HandleResult(result);
    }

    /// <summary>
    /// Get AI forecast accuracy (% of correct predictions) with filters
    /// </summary>
    [HttpGet("accuracy")]
    public async Task<IActionResult> GetAiAccuracy([FromQuery] AiForecastFilterParams filters)
    {
        var result = await _aiForecastService.GetAccuracyAsync(filters);
        if (!result.IsSuccess)
            return Problem(result.Error); 

        return Ok(new { accuracy = result.Value });
    }

    /// <summary>
    /// Get AI forecast trend (daily accuracy stats) with filtering
    /// </summary>
    [HttpGet("trend")]
    [ProducesResponseType(typeof(PagedResult<AiForecastTrendDto>), 200)]
    public async Task<IActionResult> GetTrend([FromQuery] AiForecastFilterParams filters)
    {
        var result = await _aiForecastService.GetTrendReportAsync(filters);
        if (!result.IsSuccess)
            return Problem(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Compare AI vs Classic forecast results with filtering
    /// </summary>
    [HttpGet("compare")]
    [ProducesResponseType(typeof(PagedResult<ForecastComparisonDto>), 200)]
    public async Task<IActionResult> CompareForecasts([FromQuery] AiForecastFilterParams filters)
    {
        var result = await _aiForecastService.GetForecastComparisonAsync(filters);
        if (!result.IsSuccess)
            return Problem(result.Error);

        return Ok(result.Value);
    }
    
    /// <summary>
    /// Preview AI forecast outcome probabilities for a specific match (without saving to DB)
    /// </summary>
    [HttpGet("preview/{matchId}")]
    [ProducesResponseType(typeof(AiForecastPreviewDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> PreviewForecast(int matchId)
    {
        var result = await _aiForecastService.PreviewForecastAsync(matchId);
    
        if (result == null || result.AllOutcomePredictions == null || !result.AllOutcomePredictions.Any())
            return NotFound(new ProblemDetails { Title = "No forecast predictions available for this match." });

        return Ok(result);
    }
    
    /// <summary>
    /// Generate AI forecasts for all matches of a given season
    /// </summary>
    [HttpPost("generate/season/{seasonId}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> GenerateAiForecastsForLastSeason(int seasonId)
    {
        var result = await _aiForecastService.GenerateAiForecastForLastSeasonAsync(seasonId);
        return HandleResult(result);
    }
}
