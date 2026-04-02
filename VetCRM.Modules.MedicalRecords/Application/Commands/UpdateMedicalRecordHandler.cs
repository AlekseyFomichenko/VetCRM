using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.MedicalRecords.Application.Commands
{
    public sealed class UpdateMedicalRecordHandler(IMedicalRecordRepository repository)
    {
        private readonly IMedicalRecordRepository _repository = repository;

        public async Task Handle(UpdateMedicalRecordCommand command, CancellationToken ct)
        {
            MedicalRecord? record = await _repository.GetByIdAsync(command.Id, ct);
            if (record is null)
                throw new MedicalRecordNotFoundException(command.Id);

            record.Update(
                command.Complaint,
                command.Diagnosis,
                command.TreatmentPlan,
                command.Prescription,
                command.Attachments);

            await _repository.SaveAsync(ct);
        }
    }
}
