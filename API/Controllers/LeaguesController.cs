using Application.Core;
using Application.DTOs.Leagues;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LeaguesController : BaseApiController
{
    private readonly ILeagueService _leagueService;

    public LeaguesController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    /// <summary>
    /// Get all leagues with optional filters and pagination
    /// </summary>
    /// <param name="filters">Filter parameters (countryId, competitionId, name, pageNumber, pageSize)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LeagueDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] LeagueFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _leagueService.GetAllAsync(filters, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a league by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LeagueDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _leagueService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new league (and optionally a competition)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LeagueDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Create([FromBody] CreateLeagueDto dto)
    {
        var result = await _leagueService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Update a league
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Update([FromBody] UpdateLeagueDto dto)
    {
        var result = await _leagueService.UpdateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a league (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _leagueService.DeleteAsync(id);
        return HandleResult(result);
    }
}
