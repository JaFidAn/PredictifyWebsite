using Domain.Entities;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace Persistence.Extensions;

public static class AuditExtensions
{
    public static List<AuditLog> GenerateAuditLogs(this ChangeTracker changeTracker, string? userId, string? ipAddress, string? browserInfo, string? endpoint)
    {
        var auditLogs = new List<AuditLog>();
        var now = DateTime.UtcNow;

        foreach (var entry in changeTracker.Entries().Where(e =>
                     e.Entity is BaseEntity &&
                     (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)))
        {
            var entity = (BaseEntity)entry.Entity;

            entity.UpdatedAt = now;
            entity.UpdatedBy = userId;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
                entity.CreatedBy = userId;
            }

            var idProp = entry.Properties.FirstOrDefault(p =>
                p.Metadata.IsPrimaryKey() && p.Metadata.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

            var recordId = idProp?.CurrentValue?.ToString()
                         ?? entry.OriginalValues.Properties
                                .Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                                .Select(p => entry.OriginalValues[p.Name]?.ToString())
                                .FirstOrDefault()
                         ?? Guid.NewGuid().ToString();

            var tableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name;

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid().ToString(),
                Action = entry.State switch
                {
                    EntityState.Added => "Create",
                    EntityState.Modified => "Update",
                    EntityState.Deleted => "Delete",
                    _ => "Unknown"
                },
                TableName = tableName,
                RecordId = recordId,
                UserId = userId ?? "System",
                Timestamp = now,
                IPAddress = ipAddress,
                BrowserInfo = browserInfo,
                Endpoint = endpoint,
                OldValues = entry.State is EntityState.Modified or EntityState.Deleted
                    ? JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p.Name]))
                    : null,
                NewValues = entry.State is EntityState.Added or EntityState.Modified
                    ? JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p.Name]))
                    : null
            };

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }
}
