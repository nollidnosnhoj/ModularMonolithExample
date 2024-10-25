using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks;

public interface IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);
    public void AddSchema(IRequestExecutorBuilder builder, IConfiguration configuration, IHostEnvironment environment);
}