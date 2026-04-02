namespace VetCRM.SharedKernel
{
    public sealed class MedicalRecordNotFoundException : DomainException
    {
        public Guid MedicalRecordId { get; }

        public MedicalRecordNotFoundException(Guid medicalRecordId)
            : base($"Medical record with id '{medicalRecordId}' was not found.")
        {
            MedicalRecordId = medicalRecordId;
        }
    }
}
