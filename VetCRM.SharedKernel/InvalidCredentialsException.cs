namespace VetCRM.SharedKernel
{
    public sealed class InvalidCredentialsException : DomainException
    {
        public InvalidCredentialsException() : base("Invalid email or password.")
        {
        }
    }
}
