using Consultation.API.Commands;
using Consultation.API.Dtos;
using HotChocolate;
using HotChocolate.Types;
using MediatR;

namespace Consultation.API;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class ConsultMutation
{
    public async Task<ConsultDto> AddConsultAsync([Service] ISender sender, AddConsultCommand input,
        CancellationToken cancellationToken = default)
    {
        var consult = await sender.Send(input, cancellationToken);
        return consult;
    }
}