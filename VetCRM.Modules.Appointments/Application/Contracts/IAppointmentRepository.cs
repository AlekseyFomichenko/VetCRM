using VetCRM.Modules.Appointments.Domain;

namespace VetCRM.Modules.Appointments.Application.Contracts
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
        Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Appointment>> GetByDateAndVetAsync(DateOnly date, Guid? vetId, CancellationToken cancellationToken);
        Task<bool> HasOverlappingForVetAsync(Guid? veterinarianUserId, DateTime startsAt, DateTime endsAt, Guid? excludeAppointmentId, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
    }
}
