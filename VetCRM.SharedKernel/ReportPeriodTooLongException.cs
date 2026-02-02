namespace VetCRM.SharedKernel
{
    public sealed class ReportPeriodTooLongException : DomainException
    {
        public ReportPeriodTooLongException(int maxDays) : base($"Report period must not exceed {maxDays} days.")
        {
        }
    }
}
