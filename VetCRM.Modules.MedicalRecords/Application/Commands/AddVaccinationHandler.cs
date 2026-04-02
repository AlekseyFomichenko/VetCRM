using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.MedicalRecords.Application.Commands
{
    public sealed class AddVaccinationHandler(IMedicalRecordRepository medicalRecordRepository, IVaccinationRepository vaccinationRepository)
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository = medicalRecordRepository;
        private readonly IVaccinationRepository _vaccinationRepository = vaccinationRepository;

        public async Task Handle(AddVaccinationCommand command, CancellationToken ct)
        {
            MedicalRecord? record = await _medicalRecordRepository.GetByIdAsync(command.MedicalRecordId, ct);
            if (record is null)
                throw new MedicalRecordNotFoundException(command.MedicalRecordId);

            Vaccination vaccination = Vaccination.Create(
                command.MedicalRecordId,
                command.VaccineName,
                command.VaccinationDate,
                command.NextDueDate,
                command.Batch,
                command.Manufacturer);

            await _vaccinationRepository.AddAsync(vaccination, ct);
            await _vaccinationRepository.SaveAsync(ct);
        }
    }
}
