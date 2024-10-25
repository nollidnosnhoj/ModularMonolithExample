using BuildingBlocks;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notes.API.Services;
using Notes.Infrastructure;
using Notes.Shared.Services;

namespace Notes.API;

public class NotesModule : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(AssemblyInfo.Assembly);
        });
        services.AddNotesDbContext(configuration);
        services.AddScoped<INoteService, NoteService>();
    }

    public void AddSchema(IRequestExecutorBuilder builder, IConfiguration configuration, IHostEnvironment environment)
    {
    }
}