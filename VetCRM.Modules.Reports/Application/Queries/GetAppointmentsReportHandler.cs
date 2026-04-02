using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Reports.Application.Queries
{
    public sealed class GetAppointmentsReportHandler(IAppointmentsForReportQuery appointmentsForReportQuery)
    {
        private const int MaxPeriodDays = 365;

        private readonly IAppointmentsForReportQuery _appointmentsForReportQuery = appointmentsForReportQuery;

        public async Task<GetAppointmentsReportResult> Handle(GetAppointmentsReportQuery query, CancellationToken ct)
        {
            var from = query.From.Date;
            var to = query.To.Date;
            if (to < from)
                throw new ArgumentException("Report period 'to' must be greater than or equal to 'from'.");

            var periodDays = (to - from).Days + 1;
            if (periodDays > MaxPeriodDays)
                throw new ReportPeriodTooLongException(MaxPeriodDays);

            var toExclusive = to.AddDays(1);

            (var items, int totalCount) = await _appointmentsForReportQuery.GetByDateRangeAsync(
                from,
                toExclusive,
                query.Page,
                query.PageSize,
                ct);

            return new GetAppointmentsReportResult(totalCount, items, query.Page, query.PageSize);
        }
    }
}
