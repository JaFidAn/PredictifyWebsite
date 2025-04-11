using Application.Repositories.AuditRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.AuditRepositories;

public class AuditLogReadRepository : ReadRepository<AuditLog, int>, IAuditLogReadRepository
{
    public AuditLogReadRepository(ApplicationDbContext context) : base(context) { }
}