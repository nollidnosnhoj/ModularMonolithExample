using BuildingBlocks.Auditing;
using BuildingBlocks.Extensions;
using Consults.Infrastructure;
using Mediator;
using ModularMonolith.API.Auditing;
using ModularMonolith.API.Extensions;
using Notes.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var hcBuilder = builder.Services
    .AddGraphQLServer()
    .AddQueryType()
    .AddMutationType()
    .AddTypeExtension<AuditLogQuery>();
builder.Services.AddAuditLogDbContext(builder.Configuration.GetDatabaseConnectionString("audit_log_db"));
builder.Services.AddMediator();

var moduleInstallers = ModuleInstallerExtensions.GetModuleInstallers(
    typeof(Notes.API.NotesModule),
    typeof(Consults.API.Module)).ToList();
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
    await app.MigrateDatabaseAsync<AuditLogDbContext>();
}

app.Run();
