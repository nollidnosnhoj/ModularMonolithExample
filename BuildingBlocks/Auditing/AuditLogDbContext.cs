using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Auditing;

public class AuditLogDbContext : DbContext
{
    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLogEntry>(entity =>
        {
            entity.ToTable("AuditLogEntries", "auditLog");
            entity.HasKey(auditLogEntry => auditLogEntry.Id);
            entity.Property(auditLogEntry => auditLogEntry.Id).ValueGeneratedNever();
            entity.Property(auditLogEntry => auditLogEntry.OccurredAt).IsRequired();
            entity.Property(auditLogEntry => auditLogEntry.Action).IsRequired();
            entity.Property(auditLogEntry => auditLogEntry.EntityType).IsRequired();
            entity.Property(auditLogEntry => auditLogEntry.EntityId).IsRequired();
            entity.Property(auditLogEntry => auditLogEntry.Payload).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
