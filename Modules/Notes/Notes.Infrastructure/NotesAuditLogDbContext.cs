using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Notes.Infrastructure;

public class NotesAuditLogDbContext : AuditLogDbContext
{
    public NotesAuditLogDbContext(DbContextOptions<NotesAuditLogDbContext> options) : base(options)
    {
    }
}
