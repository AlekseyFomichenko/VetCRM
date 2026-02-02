namespace VetCRM.Api.Controllers.MedicalRecords
{
    public sealed record VaccinationResponse(
        Guid Id,
        Guid MedicalRecordId,
        string VaccineName,
        DateTime VaccinationDate,
        DateTime? NextDueDate,
        string? Batch,
        string? Manufacturer);
}
