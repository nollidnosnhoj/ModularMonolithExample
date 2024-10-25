using Consults.API.Dtos;
using Consults.API.Queries;
using HotChocolate;
using HotChocolate.Types;

namespace Consults.API;

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