using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Domain;

namespace VetCRM.Modules.MedicalRecords.Infrastructure.Repositories
{
    public sealed class VaccinationRepository(MedicalRecordsDbContext db) : IVaccinationRepository
    {
        private readonly MedicalRecordsDbContext _db = db;

        public async Task AddAsync(Vaccination vaccination, CancellationToken cancellationToken)
        {
            await _db.Vaccinations.AddAsync(vaccination, cancellationToken);
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Vaccination>> GetByMedicalRecordIdAsync(Guid medicalRecordId, CancellationToken cancellationToken)
        {
            return await _db.Vaccinations
                .AsNoTracking()
                .Where(v => v.MedicalRecordId == medicalRecordId)
                .OrderBy(v => v.VaccinationDate)
                .ToListAsync(cancellationToken);
        }
    }
}
