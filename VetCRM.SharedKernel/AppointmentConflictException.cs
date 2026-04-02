namespace VetCRM.SharedKernel
{
    public sealed class AppointmentConflictException : DomainException
    {
        public Guid? VeterinarianUserId { get; }
        public DateTime StartsAt { get; }
        public DateTime EndsAt { get; }

        public AppointmentConflictException(Guid? veterinarianUserId, DateTime startsAt, DateTime endsAt)
            : base($"Another appointment exists for the same veterinarian in the time range {startsAt:O}–{endsAt:O}.")
        {
            VeterinarianUserId = veterinarianUserId;
            StartsAt = startsAt;
            EndsAt = endsAt;
        }
    }
}
