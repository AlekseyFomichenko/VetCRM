namespace VetCRM.Modules.MedicalRecords.Application.Commands
{
    public sealed record AddVaccinationCommand(
        Guid MedicalRecordId,
        string VaccineName,
        DateOnly VaccinationDate,
        DateOnly? NextDueDate,
        string? Batch,
        string? Manufacturer);
}
