namespace VetCRM.Modules.MedicalRecords.Application.Contracts
{
    public sealed record UpcomingVaccinationDto(
        Guid VaccinationId,
        Guid PetId,
        Guid MedicalRecordId,
        string VaccineName,
        DateOnly NextDueDate);

    public interface IUpcomingVaccinationsQuery
    {
        Task<IReadOnlyList<UpcomingVaccinationDto>> GetUpcomingAsync(
            DateOnly fromDateInclusive,
            DateOnly upToDateExclusive,
            CancellationToken cancellationToken);
    }
}
