using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Auditing;

public static class AuditLogServiceCollectionExtensions
{
    public static IServiceCollection AddModuleAuditLogDbContext<TAuditLogDbContext>(
        this IServiceCollection services,
        string connectionString,
        string migrationsHistoryTableName)
        where TAuditLogDbContext : AuditLogDbContext
    {
        services.AddDbContext<TAuditLogDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(migrationsHistoryTableName)));

        return services;
    }
}
