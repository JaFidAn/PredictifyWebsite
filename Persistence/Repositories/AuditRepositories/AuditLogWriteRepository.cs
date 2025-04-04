using Application.Repositories.AuditRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.AuditRepositories;

public class AuditLogWriteRepository : WriteRepository<AuditLog>, IAuditLogWriteRepository
{
    public AuditLogWriteRepository(ApplicationDbContext context) : base(context) { }
}