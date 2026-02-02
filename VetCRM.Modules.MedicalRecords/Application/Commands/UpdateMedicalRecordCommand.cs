namespace VetCRM.Modules.MedicalRecords.Application.Commands
{
    public sealed record UpdateMedicalRecordCommand(
        Guid Id,
        string Complaint,
        string Diagnosis,
        string TreatmentPlan,
        string Prescription,
        string? Attachments);
}
