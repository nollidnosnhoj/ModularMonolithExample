namespace BuildingBlocks.Auditing;

public class AuditLogEntry
{
    public Guid Id { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public Guid EntityId { get; set; }
    public required string Payload { get; set; }
}
