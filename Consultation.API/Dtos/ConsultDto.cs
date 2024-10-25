using Consultation.Domain.Shared;

namespace Consultation.API.Dtos;

public class ConsultDto
{
    public required Guid Id { get; init; }
    public required string PatientName { get; init; }
    public required DateTimeOffset PatientBirthDate { get; init; }
    public required Salutation PatientSalutation { get; init; }
    public required DateTimeOffset? CallDateTime { get; init; }
    public required DateTimeOffset? StartDateTime { get; init; }
    public required ConsultStatus Status { get; init; }
}