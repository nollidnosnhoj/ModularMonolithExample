using Consultation.API.Dtos;
using Consultation.API.Queries;
using HotChocolate;
using HotChocolate.Types;

namespace Consultation.API;

[ExtendObjectType(OperationTypeNames.Query)]
public class ConsultQuery
{
    public async Task<List<ConsultDto>> GetConsultsAsync(
        [Service] ConsultQueries consultQueries,
        CancellationToken cancellationToken = default)
    {
        return await consultQueries.GetConsultsAsync(cancellationToken);
    }
}