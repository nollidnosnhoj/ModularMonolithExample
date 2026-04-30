using BuildingBlocks.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Consults.Infrastructure;

public class ConsultsAuditLogDbContext : AuditLogDbContext
{
    public ConsultsAuditLogDbContext(DbContextOptions<ConsultsAuditLogDbContext> options) : base(options)
    {
    }
}
