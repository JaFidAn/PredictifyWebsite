using Application.Core;
using Application.DTOs.TeamOutcomeStreaks;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TeamOutcomeStreaksController : BaseApiController
{
    private readonly ITeamOutcomeStreakService _streakService;

    public TeamOutcomeStreaksController(ITeamOutcomeStreakService streakService)
    {
        _streakService = streakService;
    }

    /// <summary>
    /// Get all team outcome streaks with optional filters and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TeamOutcomeStreakDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] TeamOutcomeStreakFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _streakService.GetAllAsync(filters, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a team outcome streak by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TeamOutcomeStreakDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _streakService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all streaks for a specific team, ordered by MatchDate descending
    /// </summary>
    [HttpGet("by-team/{teamId}")]
    [ProducesResponseType(typeof(List<TeamOutcomeStreakDto>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetByTeamId(int teamId, CancellationToken cancellationToken)
    {
        var result = await _streakService.GetByTeamIdAsync(teamId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Recalculate streaks for a specific match
    /// </summary>
    [HttpPost("recalculate/{matchId}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> RecalculateForMatch(int matchId)
    {
        var result = await _streakService.RecalculateStreaksForMatchAsync(matchId);
        return HandleResult(result);
    }

    /// <summary>
    /// Recalculate all streaks from scratch
    /// </summary>
    [HttpPost("recalculate-all")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<IActionResult> RecalculateAll()
    {
        var result = await _streakService.RecalculateAllAsync();
        return HandleResult(result);
    }
}
