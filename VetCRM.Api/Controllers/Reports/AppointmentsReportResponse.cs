namespace VetCRM.Api.Controllers.Reports
{
    public sealed record AppointmentReportItemResponse(
        Guid Id,
        Guid PetId,
        Guid ClientId,
        Guid? VeterinarianUserId,
        DateTime StartsAt,
        DateTime EndsAt,
        int Status,
        string? Reason,
        DateTime CreatedAt);

    public sealed record AppointmentsReportResponse(
        int TotalCount,
        IReadOnlyList<AppointmentReportItemResponse> Items,
        int Page,
        int PageSize);
}
