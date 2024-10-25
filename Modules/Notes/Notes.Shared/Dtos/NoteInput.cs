using Notes.Domain.Shared;

namespace Notes.Shared.Dtos;

public class NoteInput
{
    public required string Content { get; init; }
    public required NoteType Type { get; init; }
    public List<NoteDataInput>? Data { get; init; }
}

public record NoteDataInput(string Key, string Value);