namespace VetCRM.Modules.Notifications.Domain
{
    public sealed class ReminderLog
    {
        public Guid Id { get; private set; }
        public ReminderType Type { get; private set; }
        public Guid? TargetClientId { get; private set; }
        public Guid? TargetPetId { get; private set; }
        public ReminderChannel Channel { get; private set; }
        public string Payload { get; private set; } = string.Empty;
        public ReminderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Error { get; private set; }

        private ReminderLog() { }

        private ReminderLog(
            Guid id,
            ReminderType type,
            Guid? targetClientId,
            Guid? targetPetId,
            ReminderChannel channel,
            string payload,
            ReminderStatus status,
            DateTime createdAt,
            string? error)
        {
            Id = id;
            Type = type;
            TargetClientId = targetClientId;
            TargetPetId = targetPetId;
            Channel = channel;
            Payload = payload;
            Status = status;
            CreatedAt = createdAt;
            Error = error;
        }

        public static ReminderLog Create(
            ReminderType type,
            Guid? targetClientId,
            Guid? targetPetId,
            ReminderChannel channel,
            string payload)
        {
            return new ReminderLog(
                Guid.NewGuid(),
                type,
                targetClientId,
                targetPetId,
                channel,
                payload ?? string.Empty,
                ReminderStatus.Sent,
                DateTime.UtcNow,
                null);
        }

        public void MarkFailed(string? error)
        {
            Status = ReminderStatus.Failed;
            Error = error;
        }
    }
}
