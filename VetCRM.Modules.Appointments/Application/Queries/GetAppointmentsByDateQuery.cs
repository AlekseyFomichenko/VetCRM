namespace VetCRM.Modules.Appointments.Application.Queries
{
    public sealed record GetAppointmentsByDateQuery(DateOnly Date, Guid? VetId);
}
