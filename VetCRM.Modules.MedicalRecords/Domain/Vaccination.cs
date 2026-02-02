namespace VetCRM.Modules.MedicalRecords.Domain
{
    public sealed class Vaccination
    {
        public Guid Id { get; private set; }
        public Guid MedicalRecordId { get; private set; }
        public string VaccineName { get; private set; } = string.Empty;
        public DateTime VaccinationDate { get; private set; }
        public DateTime? NextDueDate { get; private set; }
        public string? Batch { get; private set; }
        public string? Manufacturer { get; private set; }

        private Vaccination() { }

        private Vaccination(Guid id, Guid medicalRecordId, string vaccineName, DateTime vaccinationDate, DateTime? nextDueDate, string? batch, string? manufacturer)
        {
            Id = id;
            MedicalRecordId = medicalRecordId;
            VaccineName = vaccineName;
            VaccinationDate = vaccinationDate;
            NextDueDate = nextDueDate;
            Batch = batch;
            Manufacturer = manufacturer;
        }

        public static Vaccination Create(Guid medicalRecordId, string vaccineName, DateTime vaccinationDate, DateTime? nextDueDate, string? batch, string? manufacturer)
        {
            if (string.IsNullOrWhiteSpace(vaccineName))
                throw new ArgumentException("Vaccine name is required.");
            return new Vaccination(
                Guid.NewGuid(),
                medicalRecordId,
                vaccineName.Trim(),
                vaccinationDate,
                nextDueDate,
                batch?.Trim(),
                manufacturer?.Trim());
        }
    }
}
