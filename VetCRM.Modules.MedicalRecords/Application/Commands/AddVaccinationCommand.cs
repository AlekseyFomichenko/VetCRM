namespace VetCRM.Modules.MedicalRecords.Application.Commands
{
    public sealed record AddVaccinationCommand(
        Guid MedicalRecordId,
        string VaccineName,
        DateTime VaccinationDate,
        DateTime? NextDueDate,
        string? Batch,
        string? Manufacturer);
}
