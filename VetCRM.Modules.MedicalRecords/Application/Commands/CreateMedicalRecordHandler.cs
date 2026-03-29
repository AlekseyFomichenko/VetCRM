using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Domain;

namespace VetCRM.Modules.MedicalRecords.Application.Commands
{
    public sealed class CreateMedicalRecordHandler(IMedicalRecordRepository repository)
    {
        private readonly IMedicalRecordRepository _repository = repository;

        public async Task<Guid> Handle(CreateMedicalRecordCommand command, CancellationToken ct)
        {
            if (command.PetId == Guid.Empty)
                throw new ArgumentException("PetId is required.");

            MedicalRecord record = MedicalRecord.CreateFromCompletedAppointment(
                Guid.NewGuid(),
                command.PetId,
                command.VeterinarianUserId,
                command.Complaint,
                command.Diagnosis,
                command.TreatmentPlan,
                command.Prescription,
                command.Attachments);

            await _repository.AddAsync(record, ct);
            return record.Id;
        }
    }
}
