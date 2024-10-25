using BuildingBlocks;
using Consults.Infrastructure;
using Consults.API.Queries;
using Consults.API.Types;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Consults.API;

public class Module : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(AssemblyInfo.Assembly);
        });
        services.AddConsultDbContext(configuration);
        services.AddScoped<ConsultQueries>();
    }

    public void AddSchema(IRequestExecutorBuilder builder, IConfiguration configuration, IHostEnvironment environment)
    {
        builder.AddTypeExtension<ConsultQuery>()
            .AddTypeExtension<ConsultMutation>()
            .AddTypeExtension<ConsultTypeExtensions>();
    }
}