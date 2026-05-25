namespace BuildingBlocks.Extensions;

public static class ModuleInstallerExtensions
{
    public static IEnumerable<IModuleInstaller> GetModuleInstallers(params Type[] installerTypes)
    {
        return installerTypes
            .DistinctBy(type => type.FullName)
            .Where(type => typeof(IModuleInstaller).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IModuleInstaller>();
    }
}
