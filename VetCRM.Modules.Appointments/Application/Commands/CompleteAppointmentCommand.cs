namespace VetCRM.Modules.Appointments.Application.Commands
{
    public sealed record CompleteAppointmentCommand(
        Guid Id,
        string Complaint,
        string Diagnosis,
        string TreatmentPlan,
        string Prescription,
        string? Attachments);
}
