using VetCRM.Modules.MedicalRecords.Domain;

namespace VetCRM.Modules.MedicalRecords.Application.Contracts
{
    public interface IMedicalRecordRepository
    {
        Task AddAsync(MedicalRecord medicalRecord, CancellationToken cancellationToken);
        Task<MedicalRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<MedicalRecord>> GetByPetIdAsync(Guid petId, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
    }
}
