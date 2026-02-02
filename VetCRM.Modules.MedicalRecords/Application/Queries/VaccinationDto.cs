namespace VetCRM.Modules.MedicalRecords.Application.Queries
{
    public sealed record VaccinationDto(
        Guid Id,
        Guid MedicalRecordId,
        string VaccineName,
        DateTime VaccinationDate,
        DateTime? NextDueDate,
        string? Batch,
        string? Manufacturer);
}
