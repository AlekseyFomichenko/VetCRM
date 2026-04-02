namespace VetCRM.Modules.Reports.Application.Queries
{
    public sealed record GetAppointmentsReportQuery(
        DateTime From,
        DateTime To,
        int Page,
        int PageSize);
}
