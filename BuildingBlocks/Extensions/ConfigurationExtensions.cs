using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BuildingBlocks.Extensions;

public static class ConfigurationExtensions
{
    public static string GetDatabaseConnectionString(this IConfiguration configuration, string databaseName)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' was not found.");

        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = databaseName
        };

        return builder.ConnectionString;
    }
}
