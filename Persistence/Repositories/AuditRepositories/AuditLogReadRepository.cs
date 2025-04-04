using Application.Repositories.AuditRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.AuditRepositories;

public class AuditLogReadRepository : ReadRepository<AuditLog>, IAuditLogReadRepository
{
    public AuditLogReadRepository(ApplicationDbContext context) : base(context) { }
}