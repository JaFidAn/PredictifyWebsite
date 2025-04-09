using Application.Core;
using Application.DTOs.Teams;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TeamsController : BaseApiController
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    /// <summary>
    /// Get all teams with optional filters and pagination
    /// </summary>
    /// <param name="filters">Filter parameters (name, countryId, pageNumber, pageSize)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TeamDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] TeamFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _teamService.GetAllAsync(filters, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a team by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TeamDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _teamService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new team with its seasons and leagues
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TeamDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Create([FromBody] CreateTeamDto dto)
    {
        var result = await _teamService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing team and its related seasons and leagues
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Update([FromBody] UpdateTeamDto dto)
    {
        var result = await _teamService.UpdateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a team (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _teamService.DeleteAsync(id);
        return HandleResult(result);
    }
}
