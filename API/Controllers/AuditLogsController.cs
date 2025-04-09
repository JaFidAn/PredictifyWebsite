using Application.Core;
using Application.DTOs.AuditLogs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class AuditLogsController : BaseApiController
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Get all audit logs with pagination
    /// </summary>
    /// <param name="paginationParams">Pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of audit logs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var result = await _auditLogService.GetAllAsync(paginationParams, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get audit log by ID
    /// </summary>
    /// <param name="id">Audit log ID</param>
    /// <returns>Audit log entry</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AuditLogDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _auditLogService.GetByIdAsync(id);
        return HandleResult(result);
    }
}
