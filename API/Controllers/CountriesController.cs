using Application.Core;
using Application.DTOs.Countries;
using Application.Params;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CountriesController : BaseApiController
{
    private readonly ICountryService _countryService;

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    /// <summary>
    /// Get all countries with optional filters and pagination
    /// </summary>
    /// <param name="filters">Pagination + filtering parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of countries</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CountryDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAll([FromQuery] CountryFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _countryService.GetAllAsync(filters, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get country by ID
    /// </summary>
    /// <param name="id">Country ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CountryDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _countryService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new country
    /// </summary>
    /// <param name="dto">Country data</param>
    [HttpPost]
    [ProducesResponseType(typeof(CountryDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Create([FromBody] CreateCountryDto dto)
    {
        var result = await _countryService.CreateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing country
    /// </summary>
    /// <param name="dto">Country update data</param>
    [HttpPut]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Update([FromBody] UpdateCountryDto dto)
    {
        var result = await _countryService.UpdateAsync(dto);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a country by ID (soft delete)
    /// </summary>
    /// <param name="id">Country ID</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _countryService.DeleteAsync(id);
        return HandleResult(result);
    }
}
