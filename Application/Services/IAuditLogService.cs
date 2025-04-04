using Application.Core;
using Application.DTOs.AuditLogs;

namespace Application.Services;

public interface IAuditLogService
{
    Task<Result<PagedResult<AuditLogDto>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken);
    Task<Result<AuditLogDto>> GetByIdAsync(string id);
}
