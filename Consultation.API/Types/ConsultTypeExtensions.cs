using Consultation.API.Dataloaders;
using Consultation.API.Dtos;
using HotChocolate;
using HotChocolate.Types;
using Notes.Shared.Dtos;

namespace Consultation.API.Types;

[ExtendObjectType<ConsultDto>]
public class ConsultTypeExtensions
{
    public async Task<NoteDto?> GetNoteAsync([Parent] ConsultDto consult, ConsultNotesDataLoader dataLoader,
        CancellationToken cancellationToken = default)
    {
        return await dataLoader.LoadAsync(consult.Id, cancellationToken);
    }
}