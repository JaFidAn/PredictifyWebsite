using Domain.Entities;

namespace Application.Repositories.AuditRepositories;

public interface IAuditLogWriteRepository : IWriteRepository<AuditLog>
{
}