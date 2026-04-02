namespace VetCRM.Api.Controllers.Notifications
{
    public sealed record ReminderLogResponse(
        Guid Id,
        string Type,
        Guid? TargetClientId,
        Guid? TargetPetId,
        string Channel,
        string Payload,
        string Status,
        DateTime CreatedAt,
        string? Error);
}
