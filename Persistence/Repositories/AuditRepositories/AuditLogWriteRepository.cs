using Application.Repositories.AuditRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.AuditRepositories;

public class AuditLogWriteRepository : WriteRepository<AuditLog, int>, IAuditLogWriteRepository
{
    public AuditLogWriteRepository(ApplicationDbContext context) : base(context) { }
}