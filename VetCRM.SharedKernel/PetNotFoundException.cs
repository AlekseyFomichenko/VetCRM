namespace VetCRM.SharedKernel
{
    public sealed class PetNotFoundException : DomainException
    {
        public Guid PetId { get; }

        public PetNotFoundException(Guid petId)
            : base($"Pet with id '{petId}' was not found.")
        {
            PetId = petId;
        }
    }
}
