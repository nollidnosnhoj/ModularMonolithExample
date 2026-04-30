using BuildingBlocks.Auditing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Notes.Domain;
using Notes.Infrastructure;
using Notes.Shared.Commands;
using Notes.Shared.Dtos;
using System.Text.Json;

namespace Notes.API.Commands;

public class AddNoteCommandHandler : IRequestHandler<AddNoteCommand, NoteDto>
{
    private readonly NotesDbContext _dbContext;
    private readonly NotesAuditLogDbContext _auditLogDbContext;

    public AddNoteCommandHandler(NotesDbContext dbContext, NotesAuditLogDbContext auditLogDbContext)
    {
        _dbContext = dbContext;
        _auditLogDbContext = auditLogDbContext;
    }
    
    public async Task<NoteDto> Handle(AddNoteCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

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
        
        try
        {
            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _auditLogDbContext.Database.SetDbConnection(_dbContext.Database.GetDbConnection());
            await _auditLogDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
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

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        
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
