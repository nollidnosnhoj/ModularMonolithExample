using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks;

public interface IModuleInstaller
{
    /// <summary>
    /// Installs the module's services into the provided <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <param name="environment">The hosting environment.</param>
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);
    /// <summary>
    /// Adds the module's GraphQL schema to the provided <see cref="IRequestExecutorBuilder"/>.
    /// </summary>
    /// <param name="builder">The request executor builder to add the schema to.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <param name="environment">The hosting environment.</param>
    public void AddSchema(IRequestExecutorBuilder builder, IConfiguration configuration, IHostEnvironment environment);
}