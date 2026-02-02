namespace VetCRM.Modules.MedicalRecords.Application.Contracts
{
    public sealed record UpcomingVaccinationDto(
        Guid VaccinationId,
        Guid PetId,
        Guid MedicalRecordId,
        string VaccineName,
        DateTime NextDueDate);

    public interface IUpcomingVaccinationsQuery
    {
        Task<IReadOnlyList<UpcomingVaccinationDto>> GetUpcomingAsync(
            DateTime fromDateInclusive,
            DateTime upToDateExclusive,
            CancellationToken cancellationToken);
    }
}
