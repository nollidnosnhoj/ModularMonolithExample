# Building Blocks

## IModuleInstaller

```csharp
// <root>/BuildingBlocks/IModuleInstaller.cs
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
```

This is an interface used for modules to configure the IServiceCollection and the GraphQL schema builder (IRequestExecutorBuilder) for their needs.

To register the module in the monolith's startup:

```csharp
var moduleInstallers = ModuleInstallerExtensions.GetModuleInstallers(
    Notes.API.AssemblyInfo.Assembly,
    Consults.API.AssemblyInfo.Assembly
).ToList();
foreach (var moduleInstaller in moduleInstallers)
{
    moduleInstaller.Install(builder.Services, builder.Configuration, builder.Environment);
    moduleInstaller.AddSchema(hcBuilder, builder.Configuration, builder.Environment);
}
```

Notice that we are referencing the Assembly of each module. The assembly needs to be in the same package as the Module.

```csharp
// <root>/Modules/Consults/Consults.API/AssemblyInfo.cs
namespace Consults.API;

public static class AssemblyInfo
{
    public static readonly Assembly Assembly = typeof(AssemblyInfo).Assembly;
}
```

### Example

#### ./Modules/Consults/Consults.API

```csharp
// <root>/Modules/Consults/Consults.API/Module.cs
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
        builder
            .AddTypeExtension<ConsultQuery>()
            .AddTypeExtension<ConsultMutation>()
            .AddTypeExtension<ConsultTypeExtensions>();
    }
}
```

```csharp
// <root>/Modules/Consults/Consults.API/AssemblyInfo.cs
namespace Consults.API;

public static class AssemblyInfo
{
    public static readonly Assembly Assembly = typeof(AssemblyInfo).Assembly;
}
```

#### ./ModularMonolith.API

```csharp
// <root>/ModuleMonolith.API/Program.cs
var builder = WebApplication.CreateBuilder(args);
var hcBuilder = builder.Services
    .AddGraphQLServer()
    .AddQueryType()
    .AddMutationType();
var moduleInstallers = ModuleInstallerExtensions.GetModuleInstallers(
    Notes.API.AssemblyInfo.Assembly,
    Consults.API.AssemblyInfo.Assembly
).ToList();
foreach (var moduleInstaller in moduleInstallers)
{
    moduleInstaller.Install(builder.Services, builder.Configuration, builder.Environment);
    moduleInstaller.AddSchema(hcBuilder, builder.Configuration, builder.Environment);
}
var app = builder.Build();
```