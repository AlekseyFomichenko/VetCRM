namespace VetCRM.Modules.Reports.Application.Queries
{
    public sealed record OverdueVaccinationReportDto(
        Guid VaccinationId,
        Guid PetId,
        string VaccineName,
        DateOnly NextDueDate,
        bool IsOverdue,
        string? ClientFullName,
        string? ClientPhone,
        string? ClientEmail);
}
