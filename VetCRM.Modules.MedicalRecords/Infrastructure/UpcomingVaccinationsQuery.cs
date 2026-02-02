using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.MedicalRecords.Application.Contracts;

namespace VetCRM.Modules.MedicalRecords.Infrastructure
{
    public sealed class UpcomingVaccinationsQuery(MedicalRecordsDbContext db) : IUpcomingVaccinationsQuery
    {
        private readonly MedicalRecordsDbContext _db = db;

        public async Task<IReadOnlyList<UpcomingVaccinationDto>> GetUpcomingAsync(
            DateTime fromDateInclusive,
            DateTime upToDateExclusive,
            CancellationToken cancellationToken)
        {
            var list = await _db.Vaccinations
                .AsNoTracking()
                .Where(v => v.NextDueDate != null && v.NextDueDate >= fromDateInclusive && v.NextDueDate < upToDateExclusive)
                .Join(
                    _db.MedicalRecords,
                    v => v.MedicalRecordId,
                    m => m.Id,
                    (v, m) => new { v, m })
                .Select(x => new UpcomingVaccinationDto(
                    x.v.Id,
                    x.m.PetId,
                    x.m.Id,
                    x.v.VaccineName,
                    x.v.NextDueDate!.Value))
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}
