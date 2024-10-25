using MediatR;
using Notes.Domain;
using Notes.Infrastructure;
using Notes.Shared.Commands;
using Notes.Shared.Dtos;

namespace Notes.API.Commands;

public class AddNoteCommandHandler : IRequestHandler<AddNoteCommand, NoteDto>
{
    private readonly NotesDbContext _dbContext;

    public AddNoteCommandHandler(NotesDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<NoteDto> Handle(AddNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            Type = request.Type,
            OwnerType = request.OwnerType,
            OwnerId = request.OwnerId
        };
        
        _dbContext.Notes.Add(note);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new NoteDto
        {
            Id = note.Id,
            Content = note.Content,
            Type = note.Type,
            OwnerType = note.OwnerType,
            OwnerId = note.OwnerId
        };
    }
}