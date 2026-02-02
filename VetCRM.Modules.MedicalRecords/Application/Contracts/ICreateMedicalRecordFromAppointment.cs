namespace VetCRM.Modules.MedicalRecords.Application.Contracts
{
    public interface ICreateMedicalRecordFromAppointment
    {
        Task<Guid> CreateAsync(
            Guid appointmentId,
            Guid petId,
            Guid? veterinarianUserId,
            string complaint,
            string diagnosis,
            string treatmentPlan,
            string prescription,
            string? attachmentsJson,
            CancellationToken ct);
    }
}
