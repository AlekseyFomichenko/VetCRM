using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Appointments.Application.Commands
{
    public sealed class CompleteAppointmentHandler(
        IAppointmentRepository repository,
        ICreateMedicalRecordFromAppointment createMedicalRecord)
    {
        private readonly IAppointmentRepository _repository = repository;
        private readonly ICreateMedicalRecordFromAppointment _createMedicalRecord = createMedicalRecord;

        public async Task Handle(CompleteAppointmentCommand command, CancellationToken ct)
        {
            Appointment? appointment = await _repository.GetByIdAsync(command.Id, ct);
            if (appointment is null)
                throw new AppointmentNotFoundException(command.Id);

            appointment.Complete();
            await _repository.SaveAsync(ct);

            await _createMedicalRecord.CreateAsync(
                appointment.Id,
                appointment.PetId,
                appointment.VeterinarianUserId,
                command.Complaint,
                command.Diagnosis,
                command.TreatmentPlan,
                command.Prescription,
                command.Attachments,
                ct);
        }
    }
}
