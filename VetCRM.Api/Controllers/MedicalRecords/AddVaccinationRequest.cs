namespace VetCRM.Api.Controllers.MedicalRecords
{
    public sealed record AddVaccinationRequest(
        string VaccineName,
        DateOnly VaccinationDate,
        DateOnly? NextDueDate,
        string? Batch,
        string? Manufacturer);
}
