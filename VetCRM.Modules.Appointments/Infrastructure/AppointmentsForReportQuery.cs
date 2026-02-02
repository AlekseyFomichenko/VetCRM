using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;

namespace VetCRM.Modules.Appointments.Infrastructure
{
    public sealed class AppointmentsForReportQuery(AppointmentsDbContext db) : IAppointmentsForReportQuery
    {
        private readonly AppointmentsDbContext _db = db;

        public async Task<(IReadOnlyList<AppointmentReportDto> Items, int TotalCount)> GetByDateRangeAsync(
            DateTime fromInclusive,
            DateTime toExclusive,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            IQueryable<Appointment> query = _db.Appointments
                .AsNoTracking()
                .Where(a => a.StartsAt >= fromInclusive && a.StartsAt < toExclusive);

            int totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(a => a.StartsAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AppointmentReportDto(
                    a.Id,
                    a.PetId,
                    a.ClientId,
                    a.VeterinarianUserId,
                    a.StartsAt,
                    a.EndsAt,
                    (int)a.Status,
                    a.Reason,
                    a.CreatedAt))
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
