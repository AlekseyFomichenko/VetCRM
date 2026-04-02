namespace VetCRM.SharedKernel
{
    public sealed class AppointmentNotFoundException : DomainException
    {
        public Guid AppointmentId { get; }

        public AppointmentNotFoundException(Guid appointmentId)
            : base($"Appointment with id '{appointmentId}' was not found.")
        {
            AppointmentId = appointmentId;
        }
    }
}
