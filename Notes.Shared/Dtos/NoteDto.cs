using Notes.Domain.Shared;

namespace Notes.Shared.Dtos;

public class NoteDto
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required NoteType Type { get; init; }
    public required string OwnerType { get; init; }
    public required Guid OwnerId { get; init; }
    public List<NoteDataDto> Data { get; init; } = [];
}