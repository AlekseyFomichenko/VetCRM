namespace VetCRM.Modules.MedicalRecords.Application.Queries
{
    public sealed record GetMedicalRecordByIdResult(
        Guid Id,
        Guid AppointmentId,
        Guid PetId,
        Guid? VeterinarianUserId,
        string Complaint,
        string Diagnosis,
        string TreatmentPlan,
        string Prescription,
        string? Attachments,
        DateOnly CreatedAt,
        IReadOnlyList<VaccinationDto> Vaccinations);
}
