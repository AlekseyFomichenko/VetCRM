using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Domain;

namespace VetCRM.Modules.MedicalRecords.Application.Commands
{
    public sealed class CreateMedicalRecordFromAppointmentHandler(IMedicalRecordRepository repository) : ICreateMedicalRecordFromAppointment
    {
        private readonly IMedicalRecordRepository _repository = repository;

        public async Task<Guid> CreateAsync(
            Guid appointmentId,
            Guid petId,
            Guid? veterinarianUserId,
            string complaint,
            string diagnosis,
            string treatmentPlan,
            string prescription,
            string? attachmentsJson,
            CancellationToken ct)
        {
            MedicalRecord record = MedicalRecord.CreateFromCompletedAppointment(
                appointmentId,
                petId,
                veterinarianUserId,
                complaint,
                diagnosis,
                treatmentPlan,
                prescription,
                attachmentsJson);

            await _repository.AddAsync(record, ct);
            return record.Id;
        }
    }
}
