namespace VetCRM.Modules.Notifications.Application.Queries
{
    public sealed record ReminderLogDto(
        Guid Id,
        string Type,
        Guid? TargetClientId,
        Guid? TargetPetId,
        string Channel,
        string Payload,
        string Status,
        DateTime CreatedAt,
        string? Error);

    public sealed record GetReminderLogResult(IReadOnlyList<ReminderLogDto> Items);
}
