using Notes.Domain.Shared;

namespace Notes.Domain;

public class Note
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required NoteType Type { get; init; }
    public required string OwnerType { get; init; }
    public required Guid OwnerId { get; init; }
}