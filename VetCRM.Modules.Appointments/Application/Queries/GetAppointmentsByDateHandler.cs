using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;

namespace VetCRM.Modules.Appointments.Application.Queries
{
    public sealed class GetAppointmentsByDateHandler(IAppointmentRepository repository)
    {
        private readonly IAppointmentRepository _repository = repository;

        public async Task<IReadOnlyList<AppointmentDto>> Handle(GetAppointmentsByDateQuery query, CancellationToken ct)
        {
            var appointments = await _repository.GetByDateAndVetAsync(query.Date, query.VetId, ct);
            return appointments.Select(a => new AppointmentDto(
                a.Id,
                a.PetId,
                a.ClientId,
                a.VeterinarianUserId,
                a.StartsAt,
                a.EndsAt,
                a.Status,
                a.Reason,
                a.CreatedByUserId,
                a.CreatedAt)).ToList();
        }
    }
}
