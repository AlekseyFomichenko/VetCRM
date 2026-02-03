namespace VetCRM.Modules.Pets.Application.Commands
{
    public sealed record SetPetClientCommand(Guid PetId, Guid? ClientId);
}
