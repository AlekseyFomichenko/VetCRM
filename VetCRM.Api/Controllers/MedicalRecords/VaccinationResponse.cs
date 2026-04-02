namespace VetCRM.Api.Controllers.MedicalRecords
{
    public sealed record VaccinationResponse(
        Guid Id,
        Guid MedicalRecordId,
        string VaccineName,
        DateOnly VaccinationDate,
        DateOnly? NextDueDate,
        string? Batch,
        string? Manufacturer);
}
