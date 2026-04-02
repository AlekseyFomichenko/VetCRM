namespace VetCRM.Api.Controllers.MedicalRecords
{
    public sealed record UpdateMedicalRecordRequest(
        string Complaint,
        string Diagnosis,
        string TreatmentPlan,
        string Prescription,
        string? Attachments);
}
