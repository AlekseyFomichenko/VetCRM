using VetCRM.Modules.MedicalRecords.Application.Contracts;

namespace VetCRM.Modules.MedicalRecords.Application.Queries
{
    public sealed class GetMedicalRecordsByPetIdHandler(
        IMedicalRecordRepository repository,
        IVaccinationRepository vaccinationRepository)
    {
        private readonly IMedicalRecordRepository _repository = repository;
        private readonly IVaccinationRepository _vaccinationRepository = vaccinationRepository;

        public async Task<IReadOnlyList<GetMedicalRecordByIdResult>> Handle(GetMedicalRecordsByPetIdQuery query, CancellationToken ct)
        {
            var records = await _repository.GetByPetIdAsync(query.PetId, ct);
            var result = new List<GetMedicalRecordByIdResult>();
            foreach (var record in records)
            {
                var vaccinations = await _vaccinationRepository.GetByMedicalRecordIdAsync(record.Id, ct);
                var vaccinationDtos = vaccinations
                    .Select(v => new VaccinationDto(
                        v.Id,
                        v.MedicalRecordId,
                        v.VaccineName,
                        v.VaccinationDate,
                        v.NextDueDate,
                        v.Batch,
                        v.Manufacturer))
                    .ToList();

                result.Add(new GetMedicalRecordByIdResult(
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
                    vaccinationDtos));
            }
            return result;
        }
    }
}
