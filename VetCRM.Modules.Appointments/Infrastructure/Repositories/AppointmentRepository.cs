using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;

namespace VetCRM.Modules.Appointments.Infrastructure.Repositories
{
    public sealed class AppointmentRepository(AppointmentsDbContext db) : IAppointmentRepository
    {
        private readonly AppointmentsDbContext _db = db;

        public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            await _db.Appointments.AddAsync(appointment, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _db.Appointments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Appointment>> GetByDateAndVetAsync(DateOnly date, Guid? vetId, CancellationToken cancellationToken)
        {
            DateTime startOfDay = date.ToDateTime(TimeOnly.MinValue);
            DateTime endOfDay = date.ToDateTime(TimeOnly.MaxValue);

            IQueryable<Appointment> query = _db.Appointments.AsNoTracking()
                .Where(a => a.StartsAt >= startOfDay && a.StartsAt <= endOfDay);

            if (vetId.HasValue)
                query = query.Where(a => a.VeterinarianUserId == vetId.Value);

            return await query.OrderBy(a => a.StartsAt).ToListAsync(cancellationToken);
        }

        public async Task<bool> HasOverlappingForVetAsync(Guid? veterinarianUserId, DateTime startsAt, DateTime endsAt, Guid? excludeAppointmentId, CancellationToken cancellationToken)
        {
            if (!veterinarianUserId.HasValue)
                return false;

            IQueryable<Appointment> query = _db.Appointments
                .Where(a => a.VeterinarianUserId == veterinarianUserId.Value
                    && a.Status != AppointmentStatus.Cancelled
                    && a.StartsAt < endsAt
                    && a.EndsAt > startsAt);

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.Id != excludeAppointmentId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
