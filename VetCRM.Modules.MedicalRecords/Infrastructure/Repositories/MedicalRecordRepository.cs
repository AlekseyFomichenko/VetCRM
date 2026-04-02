using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Domain;

namespace VetCRM.Modules.MedicalRecords.Infrastructure.Repositories
{
    public sealed class MedicalRecordRepository(MedicalRecordsDbContext db) : IMedicalRecordRepository
    {
        private readonly MedicalRecordsDbContext _db = db;

        public async Task AddAsync(MedicalRecord medicalRecord, CancellationToken cancellationToken)
        {
            await _db.MedicalRecords.AddAsync(medicalRecord, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<MedicalRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _db.MedicalRecords.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<MedicalRecord>> GetByPetIdAsync(Guid petId, CancellationToken cancellationToken)
        {
            return await _db.MedicalRecords
                .AsNoTracking()
                .Where(m => m.PetId == petId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
