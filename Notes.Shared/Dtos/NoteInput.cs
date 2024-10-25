using Notes.Domain.Shared;

namespace Notes.Shared.Dtos;

public class NoteInput
{
    public required string Content { get; init; }
    public required NoteType Type { get; init; }
}