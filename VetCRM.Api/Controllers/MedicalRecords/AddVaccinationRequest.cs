namespace VetCRM.Api.Controllers.MedicalRecords
{
    public sealed record AddVaccinationRequest(
        string VaccineName,
        DateTime VaccinationDate,
        DateTime? NextDueDate,
        string? Batch,
        string? Manufacturer);
}
