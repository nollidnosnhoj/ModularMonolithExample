# Notes.Shared

This is the Notes module's shared package. Any module that needs data from the notes module can reference the `Notes.Shared` package.

## Example

This is the AddConsultCommandHandler.

This handler needs to reference the `Notes` module to add consult notes.

```csharp
// File: <root>/Modules/Consults/Consults.API/Commands/AddConsultCommandHandler.cs
namespace Consults.API.Commands;

public class AddConsultCommand : IRequest<ConsultDto>
{
    public required string PatientName { get; init; }
    public required DateTimeOffset PatientBirthDate { get; init; }
    public required Salutation PatientSalutation { get; init; }
    public required DateTimeOffset? CallDateTime { get; init; }
    public required DateTimeOffset? StartDateTime { get; init; }
    public required NoteInput? Note { get; set; }
}

public class AddConsultCommandHandler : IRequestHandler<AddConsultCommand, ConsultDto>
{
    // omitted for brevity

    public async Task<ConsultDto> Handle(AddConsultCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var consult = new Consult();

            _dbContext.Consults.Add(consult);
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (request.Note is not null)
            {
                // LOOK: AddNoteCommand comes from Notes.Shared reference
                var addNoteCommand = new AddNoteCommand
                {
                    Content = request.Note.Content,
                    Type = request.Note.Type,
                    OwnerType = nameof(Consult),
                    OwnerId = consult.Id,
                    Data = request.Note.Data ?? []
                };

                // LOOK: We are sending this to IMediatr
                // Because we are a monolith, there's only one mediator
                // and it knows all the commands from all modules (if installed correctly)
                await _sender.Send(addNoteCommand, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return new ConsultDto();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
```

We are referencing the Notes.Shared package in the Consult.API.csproj.

```xml
<!-- File: <root>/Modules/Consults/Consults.API/Consults.API.csproj -->
<Project Sdk="Microsoft.NET.Sdk">

    <!-- omitted for brevity -->

    <ItemGroup>
      <ProjectReference Include="..\..\..\BuildingBlocks\BuildingBlocks.csproj" />
      <ProjectReference Include="..\Consults.Domain\Consults.Domain.csproj" />
      <ProjectReference Include="..\Consults.Infrastructure\Consults.Infrastructure.csproj" />
      <!-- LOOK: We are referening the Notes.Shared module only! -->
      <ProjectReference Include="..\..\Notes\Notes.Shared\Notes.Shared.csproj" />
    </ItemGroup>

    <ItemGroup>
      <!-- omitted for brevity -->
    </ItemGroup>

</Project>
```

This class implements IRequest from IMediatr. This is the class that the AddConsultCommandHandler uses to tell the mediator to add consult notes. Because this file is in the `Notes.Shared` package, other modules (if referenced) should be able to use this class.

```csharp
// File: <root>/Modules/Notes/Notes.Shared/Commands/AddNoteCommand.cs
namespace Notes.Shared.Commands;

public class AddNoteCommand : IRequest<NoteDto>
{
    public required string Content { get; init; }
    public required NoteType Type { get; init; }
    public required string OwnerType { get; init; }
    public required Guid OwnerId { get; init; }
    public List<NoteDataInput> Data { get; init; } = [];
}
```

Here is the implementation of adding notes. This will actually add the consult notes when the AddConsultCommandHandler sends the AddNoteCommand to the mediator.

```csharp
// File: <root>/Modules/Notes/Notes.API/Commands/AddNoteCommandHandler.cs
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
```

The point of this is so that other modules do not get the implementation details about other modules. Instead, the notes module can share concrete interfaces and models that it wants to share to other modules.
