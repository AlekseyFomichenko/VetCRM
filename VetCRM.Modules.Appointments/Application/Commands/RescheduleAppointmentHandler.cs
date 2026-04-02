using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Appointments.Application.Commands
{
    public sealed class RescheduleAppointmentHandler(IAppointmentRepository repository)
    {
        private readonly IAppointmentRepository _repository = repository;

        public async Task Handle(RescheduleAppointmentCommand command, CancellationToken ct)
        {
            Appointment? appointment = await _repository.GetByIdAsync(command.Id, ct);
            if (appointment is null)
                throw new AppointmentNotFoundException(command.Id);

            bool hasOverlap = await _repository.HasOverlappingForVetAsync(
                appointment.VeterinarianUserId,
                command.StartsAt,
                command.EndsAt,
                command.Id,
                ct);
            if (hasOverlap)
                throw new AppointmentConflictException(appointment.VeterinarianUserId, command.StartsAt, command.EndsAt);

            appointment.Reschedule(command.StartsAt, command.EndsAt);
            await _repository.SaveAsync(ct);
        }
    }
}
