namespace ModularMonolith.ArchitectureTests;

public sealed record ModuleProject(
    string Name,
    string ModuleName,
    string ProjectPath,
    IReadOnlyList<ProjectReferenceInfo> ProjectReferences)
{
    public bool IsShared => Name.EndsWith(".Shared", StringComparison.Ordinal);
}

public sealed record ProjectReferenceInfo(
    string Name,
    string ProjectPath,
    string? ModuleName)
{
    public bool IsBuildingBlocks => Name == "BuildingBlocks";
    public bool IsShared => Name.EndsWith(".Shared", StringComparison.Ordinal);
    public bool IsModuleProject => ModuleName is not null;
}
