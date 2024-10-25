using Consults.Domain;
using Consults.Domain.Shared;
using Consults.Infrastructure;
using Consults.API.Dtos;
using MediatR;
using Notes.Shared.Commands;
using Notes.Shared.Dtos;

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

    public AddConsultCommandHandler(ConsultDbContext dbContext, ISender sender)
    {
        _dbContext = dbContext;
        _sender = sender;
    }

    public async Task<ConsultDto> Handle(AddConsultCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
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

            await transaction.CommitAsync(cancellationToken);
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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}