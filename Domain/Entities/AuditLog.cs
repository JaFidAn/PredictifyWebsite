using Domain.Entities.Common;

namespace Domain.Entities;

public class AuditLog : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string TableName { get; set; } = null!;
    public string RecordId { get; set; } = null!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? IPAddress { get; set; }
    public string? BrowserInfo { get; set; }
    public string? Endpoint { get; set; }
}

