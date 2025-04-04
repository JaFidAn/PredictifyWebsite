namespace Application.DTOs.AuditLogs;

public class AuditLogDto
{
    public string Id { get; set; } = null!;
    public string? UserId { get; set; }
    public string Action { get; set; } = null!;
    public string TableName { get; set; } = null!;
    public string RecordId { get; set; } = null!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime Timestamp { get; set; }

    public string? IPAddress { get; set; }
    public string? BrowserInfo { get; set; }
    public string? Endpoint { get; set; }
}
