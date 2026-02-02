namespace VetCRM.Api.Controllers.MedicalRecords
{
    public sealed record MedicalRecordResponse(
        Guid Id,
        Guid AppointmentId,
        Guid PetId,
        Guid? VeterinarianUserId,
        string Complaint,
        string Diagnosis,
        string TreatmentPlan,
        string Prescription,
        string? Attachments,
        DateTime CreatedAt,
        IReadOnlyList<VaccinationResponse> Vaccinations);
}
