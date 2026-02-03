namespace VetCRM.Api.Controllers.Reports
{
    public sealed record OverdueVaccinationItemResponse(
        Guid VaccinationId,
        Guid PetId,
        string VaccineName,
        DateOnly NextDueDate,
        bool IsOverdue,
        string? ClientFullName,
        string? ClientPhone,
        string? ClientEmail);

    public sealed record OverdueVaccinationsReportResponse(
        int TotalCount,
        IReadOnlyList<OverdueVaccinationItemResponse> Items,
        int Page,
        int PageSize);
}
