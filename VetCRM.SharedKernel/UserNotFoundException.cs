namespace VetCRM.SharedKernel
{
    public sealed class UserNotFoundException : DomainException
    {
        public Guid UserId { get; }

        public UserNotFoundException(Guid userId) : base($"User with id '{userId}' was not found.")
        {
            UserId = userId;
        }
    }
}
