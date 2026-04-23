using ViewStream.Application.Interfaces.Services;

public class AuditContext : IAuditContext
{
    public string? TableName { get; set; }
    public long? RecordId { get; set; }
    public string? Action { get; set; }
    public object? OldValues { get; set; }
    public object? NewValues { get; set; }
    public long? ChangedByUserId { get; set; }

    public bool HasData => !string.IsNullOrEmpty(TableName) && RecordId.HasValue && !string.IsNullOrEmpty(Action);

    public void Clear()
    {
        TableName = null;
        RecordId = null;
        Action = null;
        OldValues = null;
        NewValues = null;
        ChangedByUserId = null;
    }
}