using System.Reflection;

namespace BuildingBlocks.Extensions;

public static class ModuleInstallerExtensions
{
    public static IEnumerable<IModuleInstaller> GetModuleInstallers()
    {
        var entryAssembly = Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException("Could not determine the entry assembly for module discovery.");

        return CreateFromAssemblies(GetReferencedAssemblies(entryAssembly));
    }

    public static IEnumerable<IModuleInstaller> GetModuleInstallers(params Assembly[] assemblies)
    {
        return CreateFromAssemblies(assemblies);
    }

    private static IEnumerable<IModuleInstaller> CreateFromAssemblies(IEnumerable<Assembly> assemblies)
    {
        return assemblies
            .DistinctBy(assembly => assembly.FullName)
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(type => typeof(IModuleInstaller).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IModuleInstaller>();
    }

    private static IEnumerable<Assembly> GetReferencedAssemblies(Assembly entryAssembly)
    {
        var assemblies = new List<Assembly> { entryAssembly };
        var assembliesByName = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            entryAssembly.GetName().Name ?? entryAssembly.FullName ?? string.Empty
        };
        var pendingAssemblyNames = new Queue<AssemblyName>(entryAssembly.GetReferencedAssemblies());

        while (pendingAssemblyNames.TryDequeue(out var assemblyName))
        {
            if (!assembliesByName.Add(assemblyName.Name ?? assemblyName.FullName ?? string.Empty))
            {
                continue;
            }

            var assembly = Assembly.Load(assemblyName);
            assemblies.Add(assembly);

            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                pendingAssemblyNames.Enqueue(referencedAssemblyName);
            }
        }

        return assemblies;
    }
}
