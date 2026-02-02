namespace VetCRM.Modules.Reports.Application.Queries
{
    public sealed record GetOverdueVaccinationsReportResult(
        int TotalCount,
        IReadOnlyList<OverdueVaccinationReportDto> Items,
        int Page,
        int PageSize);
}
