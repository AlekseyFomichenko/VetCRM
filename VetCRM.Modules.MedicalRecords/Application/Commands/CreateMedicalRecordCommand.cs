namespace VetCRM.Modules.MedicalRecords.Application.Commands
{
    public sealed record CreateMedicalRecordCommand(
        Guid PetId,
        Guid? VeterinarianUserId,
        string Complaint,
        string Diagnosis,
        string TreatmentPlan,
        string Prescription,
        string? Attachments);
}
