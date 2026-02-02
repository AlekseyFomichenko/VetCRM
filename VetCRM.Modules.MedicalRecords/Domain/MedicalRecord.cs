namespace VetCRM.Modules.MedicalRecords.Domain
{
    public sealed class MedicalRecord
    {
        public Guid Id { get; private set; }
        public Guid AppointmentId { get; private set; }
        public Guid PetId { get; private set; }
        public Guid? VeterinarianUserId { get; private set; }
        public string Complaint { get; private set; } = string.Empty;
        public string Diagnosis { get; private set; } = string.Empty;
        public string TreatmentPlan { get; private set; } = string.Empty;
        public string Prescription { get; private set; } = string.Empty;
        public string? Attachments { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private MedicalRecord() { }

        private MedicalRecord(
            Guid id,
            Guid appointmentId,
            Guid petId,
            Guid? veterinarianUserId,
            string complaint,
            string diagnosis,
            string treatmentPlan,
            string prescription,
            string? attachments,
            DateTime createdAt)
        {
            Id = id;
            AppointmentId = appointmentId;
            PetId = petId;
            VeterinarianUserId = veterinarianUserId;
            Complaint = complaint;
            Diagnosis = diagnosis;
            TreatmentPlan = treatmentPlan;
            Prescription = prescription;
            Attachments = attachments;
            CreatedAt = createdAt;
        }

        public static MedicalRecord CreateFromCompletedAppointment(
            Guid appointmentId,
            Guid petId,
            Guid? veterinarianUserId,
            string complaint,
            string diagnosis,
            string treatmentPlan,
            string prescription,
            string? attachments)
        {
            return new MedicalRecord(
                Guid.NewGuid(),
                appointmentId,
                petId,
                veterinarianUserId,
                complaint ?? string.Empty,
                diagnosis ?? string.Empty,
                treatmentPlan ?? string.Empty,
                prescription ?? string.Empty,
                attachments,
                DateTime.UtcNow);
        }

        public void Update(string complaint, string diagnosis, string treatmentPlan, string prescription, string? attachments)
        {
            Complaint = complaint ?? string.Empty;
            Diagnosis = diagnosis ?? string.Empty;
            TreatmentPlan = treatmentPlan ?? string.Empty;
            Prescription = prescription ?? string.Empty;
            Attachments = attachments;
        }
    }
}
