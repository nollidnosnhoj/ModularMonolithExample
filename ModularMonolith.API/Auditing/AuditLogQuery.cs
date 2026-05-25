using BuildingBlocks.Auditing;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace ModularMonolith.API.Auditing;

[ExtendObjectType(OperationTypeNames.Query)]
public class AuditLogQuery
{
    public async Task<List<AuditLogDto>> GetAuditLogsAsync(
        [Service] AuditLogDbContext dbContext,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 500);

        return await dbContext.AuditLogEntries
            .TagWith("GetAuditLogsQuery")
            .OrderByDescending(auditLogEntry => auditLogEntry.OccurredAt)
            .Take(take)
            .Select(auditLogEntry => new AuditLogDto
            {
                Id = auditLogEntry.Id,
                OccurredAt = auditLogEntry.OccurredAt,
                Action = auditLogEntry.Action,
                EntityType = auditLogEntry.EntityType,
                EntityId = auditLogEntry.EntityId,
                Payload = auditLogEntry.Payload
            })
            .ToListAsync(cancellationToken);
    }
}
