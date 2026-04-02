using VetCRM.Modules.MedicalRecords.Domain;

namespace VetCRM.Modules.MedicalRecords.Application.Contracts
{
    public interface IVaccinationRepository
    {
        Task AddAsync(Vaccination vaccination, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<Vaccination>> GetByMedicalRecordIdAsync(Guid medicalRecordId, CancellationToken cancellationToken);
    }
}
