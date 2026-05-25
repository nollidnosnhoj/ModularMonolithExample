namespace ModularMonolith.API.Auditing;

public class AuditLogDto
{
    public Guid Id { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public required string Action { get; init; }
    public required string EntityType { get; init; }
    public Guid EntityId { get; init; }
    public required string Payload { get; init; }
}
