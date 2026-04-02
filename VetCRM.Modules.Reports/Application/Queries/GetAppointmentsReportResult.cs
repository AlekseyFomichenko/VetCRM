using VetCRM.Modules.Appointments.Application.Contracts;

namespace VetCRM.Modules.Reports.Application.Queries
{
    public sealed record GetAppointmentsReportResult(
        int TotalCount,
        IReadOnlyList<AppointmentReportDto> Items,
        int Page,
        int PageSize);
}
