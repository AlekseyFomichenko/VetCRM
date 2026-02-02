using VetCRM.Modules.Appointments.Domain;

namespace VetCRM.Api.Controllers.Appointments
{
    public sealed record AppointmentResponse(
        Guid Id,
        Guid PetId,
        Guid ClientId,
        Guid? VeterinarianUserId,
        DateTime StartsAt,
        DateTime EndsAt,
        AppointmentStatus Status,
        string? Reason,
        Guid? CreatedByUserId,
        DateTime CreatedAt);
}
