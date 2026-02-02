namespace VetCRM.Modules.Appointments.Application.Commands
{
    public sealed record RescheduleAppointmentCommand(Guid Id, DateTime StartsAt, DateTime EndsAt);
}
