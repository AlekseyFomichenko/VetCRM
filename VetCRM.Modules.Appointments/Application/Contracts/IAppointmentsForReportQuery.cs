namespace VetCRM.Modules.Appointments.Application.Contracts
{
    public sealed record AppointmentReportDto(
        Guid Id,
        Guid PetId,
        Guid ClientId,
        Guid? VeterinarianUserId,
        DateTime StartsAt,
        DateTime EndsAt,
        int Status,
        string? Reason,
        DateTime CreatedAt);

    public interface IAppointmentsForReportQuery
    {
        Task<(IReadOnlyList<AppointmentReportDto> Items, int TotalCount)> GetByDateRangeAsync(
            DateTime fromInclusive,
            DateTime toExclusive,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
