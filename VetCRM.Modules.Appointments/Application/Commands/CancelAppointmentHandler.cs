using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Appointments.Application.Commands
{
    public sealed class CancelAppointmentHandler(IAppointmentRepository repository)
    {
        private readonly IAppointmentRepository _repository = repository;

        public async Task Handle(CancelAppointmentCommand command, CancellationToken ct)
        {
            Appointment? appointment = await _repository.GetByIdAsync(command.Id, ct);
            if (appointment is null)
                throw new AppointmentNotFoundException(command.Id);

            appointment.Cancel();
            await _repository.SaveAsync(ct);
        }
    }
}
