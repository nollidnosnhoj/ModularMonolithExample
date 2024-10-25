using BuildingBlocks;
using Consultation.API.Queries;
using Consultation.API.Types;
using Consultation.Infrastructure;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Consultation.API;

public class ConsultationModule : IModuleInstaller
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