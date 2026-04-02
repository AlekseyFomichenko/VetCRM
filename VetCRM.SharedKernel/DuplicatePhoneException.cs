namespace VetCRM.SharedKernel
{
    public sealed class DuplicatePhoneException : DomainException
    {
        public string Phone { get; }

        public DuplicatePhoneException(string phone)
            : base($"Client with phone '{phone}' already exists.")
        {
            Phone = phone;
        }
    }
}
