using Notes.Domain.Shared;
using Notes.Infrastructure;
using Notes.Shared.Dtos;
using Notes.Shared.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Notes.API.Services;

public class NoteService : INoteService
{
    private readonly NotesDbContext _dbContext;

    public NoteService(NotesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<NoteDto>> GetNotesByOwnerIdsAsync(IEnumerable<Guid> ownerIds, 
        string? ownerType = null, 
        NoteType? type = null,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Notes.AsQueryable()
            .Where(note => ownerIds.Contains(note.OwnerId));
        
        if (ownerType is not null)
        {
            queryable = queryable.Where(note => note.OwnerType == ownerType);
        }
        
        if (type is not null)
        {
            queryable = queryable.Where(note => note.Type == type);
        }
        
        return await queryable
            .Select(note => new NoteDto
            {
                Id = note.Id,
                Content = note.Content,
                Type = note.Type,
                OwnerType = note.OwnerType,
                OwnerId = note.OwnerId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<NoteDto>> GetNotesByOwnerTypeAsync(string ownerType, NoteType? type = null, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Notes.AsQueryable()
            .Where(note => note.OwnerType == ownerType);
        
        if (type is not null)
        {
            queryable = queryable.Where(note => note.Type == type);
        }
        
        return await queryable
            .Select(note => new NoteDto
            {
                Id = note.Id,
                Content = note.Content,
                Type = note.Type,
                OwnerType = note.OwnerType,
                OwnerId = note.OwnerId
            })
            .ToListAsync(cancellationToken);
    }
}