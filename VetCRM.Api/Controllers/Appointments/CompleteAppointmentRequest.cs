namespace VetCRM.Api.Controllers.Appointments
{
    public sealed record CompleteAppointmentRequest(
        string Complaint,
        string Diagnosis,
        string TreatmentPlan,
        string Prescription,
        string? Attachments);
}
