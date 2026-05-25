using BuildingBlocks.Auditing;
using Consults.Domain;
using Consults.Domain.Shared;
using Consults.Infrastructure;
using Consults.API.Dtos;
using Mediator;
using Notes.Shared.Commands;
using Notes.Shared.Dtos;
using System.Text.Json;

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
    private readonly ISender _sender;
    private readonly ConsultDbContext _dbContext;
    private readonly AuditLogDbContext _auditLogDbContext;

    public AddConsultCommandHandler(
        ConsultDbContext dbContext,
        AuditLogDbContext auditLogDbContext,
        ISender sender)
    {
        _dbContext = dbContext;
        _auditLogDbContext = auditLogDbContext;
        _sender = sender;
    }

    public async ValueTask<ConsultDto> Handle(AddConsultCommand request, CancellationToken cancellationToken)
    {
        var consult = new Consult
        {
            Id = Guid.NewGuid(),
            PatientName = request.PatientName,
            PatientBirthDate = request.PatientBirthDate,
            PatientSalutation = request.PatientSalutation,
            CallDateTime = request.CallDateTime,
            StartDateTime = request.StartDateTime,
            Status = ConsultStatus.Draft
        };

        _dbContext.Consults.Add(consult);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _auditLogDbContext.AuditLogEntries.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow,
            Action = "Created",
            EntityType = nameof(Consult),
            EntityId = consult.Id,
            Payload = JsonSerializer.Serialize(request)
        });
        await _auditLogDbContext.SaveChangesAsync(cancellationToken);

        if (request.Note is not null)
        {
            var addNoteCommand = new AddNoteCommand
            {
                Content = request.Note.Content,
                Type = request.Note.Type,
                OwnerType = nameof(Consult),
                OwnerId = consult.Id,
                Data = request.Note.Data ?? []
            };

            await _sender.Send(addNoteCommand, cancellationToken);
        }

        return new ConsultDto
        {
            Id = consult.Id,
            PatientName = consult.PatientName,
            PatientBirthDate = consult.PatientBirthDate,
            PatientSalutation = consult.PatientSalutation,
            CallDateTime = consult.CallDateTime,
            StartDateTime = consult.StartDateTime,
            Status = consult.Status
        };
    }
}
