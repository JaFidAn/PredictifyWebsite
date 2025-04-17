using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ImportController : BaseApiController
{
    private readonly IFootballDataImporterService _importerService;

    public ImportController(IFootballDataImporterService importerService)
    {
        _importerService = importerService;
    }

    /// <summary>
    /// Imports initial football data (countries, leagues, teams) from JSON files.
    /// </summary>
    [HttpPost("initial-data")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> ImportInitialData()
    {
        try
        {
            await _importerService.ImportInitialDataAsync(CancellationToken.None);
            return Ok(true);
        }
        catch (Exception ex)
        {
            return Problem(title: "Import failed", detail: ex.Message, statusCode: 500);
        }
    }

    /// <summary>
    /// Imports match data (with scores) from JSON files.
    /// </summary>
    [HttpPost("matches")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> ImportMatches()
    {
        try
        {
            await _importerService.ImportMatchesAsync(CancellationToken.None);
            return Ok(true);
        }
        catch (Exception ex)
        {
            return Problem(title: "Match import failed", detail: ex.Message, statusCode: 500);
        }
    }
}