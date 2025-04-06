using Application.Core;
using Application.DTOs.Seasons;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class SeasonsController : BaseApiController
{
    private readonly ISeasonService _seasonService;

    public SeasonsController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    /// <summary>
    /// Get all seasons with optional filters and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SeasonDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] SeasonFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _seasonService.GetAllAsync(filters, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a season by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SeasonDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _seasonService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new season
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SeasonDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Create([FromBody] CreateSeasonDto dto)
    {
        var result = await _seasonService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing season
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Update([FromBody] UpdateSeasonDto dto)
    {
        var result = await _seasonService.UpdateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a season (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _seasonService.DeleteAsync(id);
        return HandleResult(result);
    }
}
