using Application.Core;
using Application.DTOs.Matches;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MatchesController : BaseApiController
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    /// <summary>
    /// Get all matches with optional filters and pagination
    /// </summary>
    /// <param name="filters">Filter parameters (teamId, pageNumber, pageSize)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MatchDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] MatchFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _matchService.GetAllAsync(filters, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a match by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MatchDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _matchService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new match with outcomes and season/league mappings
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MatchDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
    {
        var result = await _matchService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing match with new outcomes and mappings
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Update([FromBody] UpdateMatchDto dto)
    {
        var result = await _matchService.UpdateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a match (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _matchService.DeleteAsync(id);
        return HandleResult(result);
    }
}
