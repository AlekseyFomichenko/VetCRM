namespace VetCRM.Api.Controllers.Appointments
{
    public sealed record RescheduleAppointmentRequest(DateTime StartsAt, DateTime EndsAt);
}
