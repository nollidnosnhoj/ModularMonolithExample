using Consults.Domain;
using GreenDonut;
using Notes.Shared.Dtos;
using Notes.Shared.Services;

namespace Consults.API.Dataloaders;

public class ConsultNotesDataLoader : BatchDataLoader<Guid, NoteDto>
{
    private readonly INoteService _noteService;
    public ConsultNotesDataLoader(IBatchScheduler batchScheduler, DataLoaderOptions options, INoteService noteService) 
        : base(batchScheduler, options)
    {
        _noteService = noteService;
    }

    protected override async Task<IReadOnlyDictionary<Guid, NoteDto>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        var notes = await _noteService.GetNotesByOwnerIdsAsync(
            keys,
            ownerType: nameof(Consult), 
            cancellationToken: cancellationToken);
        
        return notes.ToDictionary(note => note.OwnerId);
    }
}