using Application.Core;
using Application.DTOs.Outcomes;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OutcomesController : BaseApiController
{
    private readonly IOutcomeService _outcomeService;

    public OutcomesController(IOutcomeService outcomeService)
    {
        _outcomeService = outcomeService;
    }

    /// <summary>
    /// Get all outcomes with optional filters and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OutcomeDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] OutcomeFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _outcomeService.GetAllAsync(filters, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get outcome by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OutcomeDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _outcomeService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new outcome
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OutcomeDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Create([FromBody] CreateOutcomeDto dto)
    {
        var result = await _outcomeService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing outcome
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Update([FromBody] UpdateOutcomeDto dto)
    {
        var result = await _outcomeService.UpdateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete an outcome (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _outcomeService.DeleteAsync(id);
        return HandleResult(result);
    }
}
