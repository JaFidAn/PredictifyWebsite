using Application.Core;
using Application.DTOs.AuditLogs;
using Application.Repositories.AuditRepositories;
using Application.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogReadRepository _readRepository;
    private readonly IMapper _mapper;

    public AuditLogService(IAuditLogReadRepository readRepository, IMapper mapper)
    {
        _readRepository = readRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var query = _readRepository
            .GetAll()
            .OrderByDescending(x => x.Timestamp)
            .ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider);

        var pagedResult = await PagedResult<AuditLogDto>.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            cancellationToken
        );

        return Result<PagedResult<AuditLogDto>>.Success(pagedResult);
    }

    public async Task<Result<AuditLogDto>> GetByIdAsync(string id)
    {
        var log = await _readRepository
            .GetAll()
            .Where(x => x.Id == id)
            .ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (log == null)
            return Result<AuditLogDto>.Failure("Audit log not found", 404);

        return Result<AuditLogDto>.Success(log);
    }
}
