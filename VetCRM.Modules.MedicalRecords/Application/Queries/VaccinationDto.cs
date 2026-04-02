namespace VetCRM.Modules.MedicalRecords.Application.Queries
{
    public sealed record VaccinationDto(
        Guid Id,
        Guid MedicalRecordId,
        string VaccineName,
        DateOnly VaccinationDate,
        DateOnly? NextDueDate,
        string? Batch,
        string? Manufacturer);
}
