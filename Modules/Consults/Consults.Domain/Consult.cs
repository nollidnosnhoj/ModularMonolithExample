using Consults.Domain.Shared;

namespace Consults.Domain;

public class Consult
{
    public required Guid Id { get; init; }
    public required string PatientName { get; init; }
    public required DateTimeOffset PatientBirthDate { get; init; }
    public required Salutation PatientSalutation { get; init; }
    public required DateTimeOffset? CallDateTime { get; init; }
    public required DateTimeOffset? StartDateTime { get; init; }
    public ConsultStatus Status { get; init; } = ConsultStatus.Draft;
}