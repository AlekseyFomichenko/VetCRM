using VetCRM.Modules.Appointments.Domain;

namespace VetCRM.Modules.Appointments.Application.Queries
{
    public sealed record AppointmentDto(
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
