using Notes.Domain.Shared;
using Notes.Shared.Dtos;

namespace Notes.Shared.Services;

public interface INoteService
{
    Task<List<NoteDto>> GetNotesByOwnerIdsAsync(
        IEnumerable<Guid> ownerIds,
        string? ownerType = null,
        NoteType? type = null,
        CancellationToken cancellationToken = default);
    Task<List<NoteDto>> GetNotesByOwnerTypeAsync(
        string ownerType,
        NoteType? type = null,
        CancellationToken cancellationToken = default);
}