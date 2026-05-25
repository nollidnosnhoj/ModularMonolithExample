using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Auditing;

public static class AuditLogServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLogDbContext(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AuditLogDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory_AuditLog", "auditLog")));

        return services;
    }
}
