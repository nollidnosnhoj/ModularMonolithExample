using BuildingBlocks.Auditing;
using Mediator;
using Notes.Domain;
using Notes.Infrastructure;
using Notes.Shared.Commands;
using Notes.Shared.Dtos;
using System.Text.Json;

namespace Notes.API.Commands;

public class AddNoteCommandHandler : IRequestHandler<AddNoteCommand, NoteDto>
{
    private readonly NotesDbContext _dbContext;
    private readonly AuditLogDbContext _auditLogDbContext;

    public AddNoteCommandHandler(NotesDbContext dbContext, AuditLogDbContext auditLogDbContext)
    {
        _dbContext = dbContext;
        _auditLogDbContext = auditLogDbContext;
    }
    
    public async ValueTask<NoteDto> Handle(AddNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            Type = request.Type,
            OwnerType = request.OwnerType,
            OwnerId = request.OwnerId,
            Data = request.Data.Select(data => new NoteData
            {
                Id = Guid.NewGuid(),
                Key = data.Key,
                Value = data.Value
            }).ToList()
        };
        
        _dbContext.Notes.Add(note);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _auditLogDbContext.AuditLogEntries.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow,
            Action = "Created",
            EntityType = nameof(Note),
            EntityId = note.Id,
            Payload = JsonSerializer.Serialize(request)
        });
        await _auditLogDbContext.SaveChangesAsync(cancellationToken);
        
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
