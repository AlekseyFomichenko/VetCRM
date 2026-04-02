namespace VetCRM.Modules.Appointments.Application.Commands
{
    public sealed record CreateAppointmentCommand(
        Guid PetId,
        Guid ClientId,
        Guid? VeterinarianUserId,
        DateTime StartsAt,
        DateTime EndsAt,
        string? Reason);
}
