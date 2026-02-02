namespace VetCRM.Modules.Appointments.Domain
{
    public sealed class Appointment
    {
        public Guid Id { get; private set; }
        public Guid PetId { get; private set; }
        public Guid ClientId { get; private set; }
        public Guid? VeterinarianUserId { get; private set; }
        public DateTime StartsAt { get; private set; }
        public DateTime EndsAt { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public string? Reason { get; private set; }
        public Guid? CreatedByUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Appointment() { }

        private Appointment(
            Guid id,
            Guid petId,
            Guid clientId,
            Guid? veterinarianUserId,
            DateTime startsAt,
            DateTime endsAt,
            string? reason,
            Guid? createdByUserId,
            DateTime createdAt)
        {
            Id = id;
            PetId = petId;
            ClientId = clientId;
            VeterinarianUserId = veterinarianUserId;
            StartsAt = startsAt;
            EndsAt = endsAt;
            Status = AppointmentStatus.Scheduled;
            Reason = reason;
            CreatedByUserId = createdByUserId;
            CreatedAt = createdAt;
        }

        public static Appointment Create(
            Guid petId,
            Guid clientId,
            Guid? veterinarianUserId,
            DateTime startsAt,
            DateTime endsAt,
            string? reason,
            Guid? createdByUserId)
        {
            if (petId == Guid.Empty)
                throw new ArgumentException("PetId is required.");
            if (clientId == Guid.Empty)
                throw new ArgumentException("ClientId is required.");
            if (endsAt <= startsAt)
                throw new ArgumentException("EndsAt must be after StartsAt.");

            return new Appointment(
                Guid.NewGuid(),
                petId,
                clientId,
                veterinarianUserId,
                startsAt,
                endsAt,
                reason?.Trim(),
                createdByUserId,
                DateTime.UtcNow);
        }

        public void Reschedule(DateTime startsAt, DateTime endsAt)
        {
            if (Status != AppointmentStatus.Scheduled && Status != AppointmentStatus.Rescheduled)
                throw new InvalidOperationException("Only scheduled or rescheduled appointments can be rescheduled.");
            if (endsAt <= startsAt)
                throw new ArgumentException("EndsAt must be after StartsAt.");

            StartsAt = startsAt;
            EndsAt = endsAt;
            Status = AppointmentStatus.Rescheduled;
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Appointment is already cancelled.");
            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Completed appointment cannot be cancelled.");

            Status = AppointmentStatus.Cancelled;
        }

        public void Complete()
        {
            if (Status != AppointmentStatus.Scheduled && Status != AppointmentStatus.Rescheduled)
                throw new InvalidOperationException("Only scheduled or rescheduled appointments can be completed.");

            Status = AppointmentStatus.Completed;
        }
    }
}
