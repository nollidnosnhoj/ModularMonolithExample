using System.Xml.Linq;

namespace ModularMonolith.ArchitectureTests;

internal static class ProjectReferenceReader
{
    public static IReadOnlyList<ModuleProject> ReadModuleProjects()
    {
        var repositoryRoot = FindRepositoryRoot();
        var modulesDirectory = Path.Combine(repositoryRoot, "Modules");
        var moduleProjectPaths = Directory
            .EnumerateFiles(modulesDirectory, "*.csproj", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();

        return moduleProjectPaths
            .Select(path => CreateModuleProject(path, repositoryRoot))
            .ToList();
    }

    private static ModuleProject CreateModuleProject(string projectPath, string repositoryRoot)
    {
        var document = XDocument.Load(projectPath);
        var projectReferences = document
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath)!, include!), repositoryRoot))
            .Select(CreateProjectReferenceInfo)
            .ToList();

        var projectName = Path.GetFileNameWithoutExtension(projectPath);
        var moduleName = GetModuleName(projectPath);

        return new ModuleProject(projectName, moduleName, projectPath, projectReferences);
    }

    private static ProjectReferenceInfo CreateProjectReferenceInfo(string projectPath)
    {
        var projectName = Path.GetFileNameWithoutExtension(projectPath);
        return new ProjectReferenceInfo(projectName, projectPath, TryGetModuleName(projectPath));
    }

    private static string FindRepositoryRoot()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDirectory is not null)
        {
            var solutionPath = Path.Combine(currentDirectory.FullName, "ModularMonolith.sln");
            if (File.Exists(solutionPath))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("Could not locate the repository root.");
    }

    private static string GetModuleName(string projectPath)
    {
        return TryGetModuleName(projectPath)
            ?? throw new InvalidOperationException($"Project '{projectPath}' is not inside the Modules directory.");
    }

    private static string? TryGetModuleName(string projectPath)
    {
        var normalizedPath = projectPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        var moduleMarker = $"{Path.DirectorySeparatorChar}Modules{Path.DirectorySeparatorChar}";
        var markerIndex = normalizedPath.IndexOf(moduleMarker, StringComparison.Ordinal);
        if (markerIndex < 0)
        {
            return null;
        }

        var relativePath = normalizedPath[(markerIndex + moduleMarker.Length)..];
        var pathParts = relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        return pathParts.Length > 0 ? pathParts[0] : null;
    }
}
