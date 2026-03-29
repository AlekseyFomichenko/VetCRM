namespace VetCRM.Api.Controllers.MedicalRecords
{
    public sealed record CreateMedicalRecordRequest(
        string Complaint,
        string Diagnosis,
        string TreatmentPlan,
        string Prescription,
        string? Attachments);
}
