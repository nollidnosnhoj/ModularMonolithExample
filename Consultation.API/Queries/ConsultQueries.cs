using Consultation.API.Dtos;
using Consultation.Domain;
using Consultation.Infrastructure;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using Notes.Shared.Dtos;
using Notes.Shared.Services;

namespace Consultation.API.Queries;

public class ConsultQueries
{
    private readonly ConsultDbContext _dbContext;

    public ConsultQueries(ConsultDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<List<ConsultDto>> GetConsultsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Consults
            .Select(consult => new ConsultDto
            {
                Id = consult.Id,
                PatientName = consult.PatientName,
                PatientBirthDate = consult.PatientBirthDate,
                PatientSalutation = consult.PatientSalutation,
                CallDateTime = consult.CallDateTime,
                StartDateTime = consult.StartDateTime,
                Status = consult.Status
            })
            .ToListAsync(cancellationToken);
    }
}