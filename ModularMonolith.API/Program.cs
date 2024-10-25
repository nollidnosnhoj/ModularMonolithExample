using BuildingBlocks.Extensions;
using Consultation.Infrastructure;
using ModularMonolith.API.Extensions;
using Notes.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var hcBuilder = builder.Services
    .AddGraphQLServer()
    .AddQueryType()
    .AddMutationType();
var moduleInstallers = ModuleInstallerExtensions.GetModuleInstallers(
    Notes.API.AssemblyInfo.Assembly,
    Consultation.API.AssemblyInfo.Assembly).ToList();
foreach (var moduleInstaller in moduleInstallers)
{
    moduleInstaller.Install(builder.Services, builder.Configuration, builder.Environment);
    moduleInstaller.AddSchema(hcBuilder, builder.Configuration, builder.Environment);
}
var app = builder.Build();

app.MapGraphQL();

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabaseAsync<NotesDbContext>();
    await app.MigrateDatabaseAsync<ConsultDbContext>();
}

app.Run();