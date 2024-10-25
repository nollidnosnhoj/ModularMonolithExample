using System.Reflection;

namespace BuildingBlocks.Extensions;

public static class ModuleInstallerExtensions
{
    public static IEnumerable<IModuleInstaller> GetModuleInstallers(params Assembly[] assemblies)
    {
        var moduleInstallers = CreateFromAssemblies<IModuleInstaller>(assemblies);
        return moduleInstallers;
    }

    private static IEnumerable<T> CreateFromAssemblies<T>(IEnumerable<Assembly> assemblies)
    {
        return assemblies
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(type => typeof(T).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<T>();
    }
}