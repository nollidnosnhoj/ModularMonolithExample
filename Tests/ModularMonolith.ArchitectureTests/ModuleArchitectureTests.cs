using Xunit;

namespace ModularMonolith.ArchitectureTests;

public class ModuleArchitectureTests
{
    [Fact]
    public void Module_projects_should_only_reference_other_module_projects_or_building_blocks()
    {
        var projects = ProjectReferenceReader.ReadModuleProjects();

        var invalidReferences = projects
            .SelectMany(project => project.ProjectReferences.Select(reference => new { project, reference }))
            .Where(x => !x.reference.IsBuildingBlocks && !x.reference.IsModuleProject)
            .Select(x => $"{x.project.Name} references '{x.reference.Name}', which is not a module project or BuildingBlocks.")
            .ToList();

        Assert.True(invalidReferences.Count == 0, string.Join(Environment.NewLine, invalidReferences));
    }

    [Fact]
    public void Module_projects_should_only_reference_other_modules_through_shared_projects()
    {
        var projects = ProjectReferenceReader.ReadModuleProjects();

        var invalidReferences = projects
            .SelectMany(project => project.ProjectReferences.Select(reference => new { project, reference }))
            .Where(x => x.reference.ModuleName is not null)
            .Where(x => x.reference.ModuleName != x.project.ModuleName)
            .Where(x => !x.reference.IsShared)
            .Select(x =>
                $"{x.project.Name} references {x.reference.Name} from module {x.reference.ModuleName}, but cross-module references must target projects ending with .Shared.")
            .ToList();

        Assert.True(invalidReferences.Count == 0, string.Join(Environment.NewLine, invalidReferences));
    }

    [Fact]
    public void Non_shared_projects_should_not_be_referenced_by_other_modules()
    {
        var projects = ProjectReferenceReader.ReadModuleProjects();
        var projectsByPath = projects.ToDictionary(project => project.ProjectPath, StringComparer.OrdinalIgnoreCase);

        var invalidReferences = projects
            .SelectMany(project => project.ProjectReferences.Select(reference => new { project, reference }))
            .Where(x => x.reference.ModuleName is not null)
            .Where(x => x.reference.ModuleName != x.project.ModuleName)
            .Where(x => projectsByPath.TryGetValue(x.reference.ProjectPath, out var referencedProject) && !referencedProject.IsShared)
            .Select(x =>
                $"{x.project.Name} references non-shared project {x.reference.Name} from module {x.reference.ModuleName}.")
            .ToList();

        Assert.True(invalidReferences.Count == 0, string.Join(Environment.NewLine, invalidReferences));
    }
}
