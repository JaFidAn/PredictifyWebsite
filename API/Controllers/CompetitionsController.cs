using Application.Core;
using Application.DTOs.Competitions;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CompetitionsController : BaseApiController
{
    private readonly ICompetitionService _competitionService;

    public CompetitionsController(ICompetitionService competitionService)
    {
        _competitionService = competitionService;
    }

    /// <summary>
    /// Get all competitions with optional filters and pagination
    /// </summary>
    /// <param name="filters">Filter parameters (name, isInternational, pageNumber, pageSize)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CompetitionDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] CompetitionFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _competitionService.GetAllAsync(filters, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a competition by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompetitionDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _competitionService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new competition
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompetitionDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Create([FromBody] CreateCompetitionDto dto)
    {
        var result = await _competitionService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Update a competition
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Update([FromBody] UpdateCompetitionDto dto)
    {
        var result = await _competitionService.UpdateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a competition (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _competitionService.DeleteAsync(id);
        return HandleResult(result);
    }
}
