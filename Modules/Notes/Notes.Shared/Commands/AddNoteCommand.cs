using MediatR;
using Notes.Domain.Shared;
using Notes.Shared.Dtos;

namespace Notes.Shared.Commands;

public class AddNoteCommand : IRequest<NoteDto>
{
    public required string Content { get; init; }
    public required NoteType Type { get; init; }
    public required string OwnerType { get; init; }
    public required Guid OwnerId { get; init; }
    public List<NoteDataInput> Data { get; init; } = [];
}