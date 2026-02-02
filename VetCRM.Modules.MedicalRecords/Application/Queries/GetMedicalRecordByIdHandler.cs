using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.MedicalRecords.Application.Queries
{
    public sealed class GetMedicalRecordByIdHandler(IMedicalRecordRepository medicalRecordRepository, IVaccinationRepository vaccinationRepository)
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository = medicalRecordRepository;
        private readonly IVaccinationRepository _vaccinationRepository = vaccinationRepository;

        public async Task<GetMedicalRecordByIdResult?> Handle(GetMedicalRecordByIdQuery query, CancellationToken ct)
        {
            Domain.MedicalRecord? record = await _medicalRecordRepository.GetByIdAsync(query.Id, ct);
            if (record is null)
                return null;

            var vaccinations = await _vaccinationRepository.GetByMedicalRecordIdAsync(record.Id, ct);
            var vaccinationDtos = vaccinations.Select(v => new VaccinationDto(
                v.Id,
                v.MedicalRecordId,
                v.VaccineName,
                v.VaccinationDate,
                v.NextDueDate,
                v.Batch,
                v.Manufacturer)).ToList();

            return new GetMedicalRecordByIdResult(
                record.Id,
                record.AppointmentId,
                record.PetId,
                record.VeterinarianUserId,
                record.Complaint,
                record.Diagnosis,
                record.TreatmentPlan,
                record.Prescription,
                record.Attachments,
                record.CreatedAt,
                vaccinationDtos);
        }
    }
}
