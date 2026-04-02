namespace VetCRM.Api.Controllers.Appointments
{
    public sealed record CreateAppointmentRequest(
        Guid PetId,
        Guid ClientId,
        Guid? VeterinarianUserId,
        DateTime StartsAt,
        DateTime EndsAt,
        string? Reason);
}
