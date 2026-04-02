namespace VetCRM.SharedKernel
{
    public sealed class AccountDisabledException : DomainException
    {
        public AccountDisabledException() : base("Account is disabled.")
        {
        }
    }
}
