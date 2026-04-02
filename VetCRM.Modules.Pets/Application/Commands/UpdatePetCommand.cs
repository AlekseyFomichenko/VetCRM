namespace VetCRM.Modules.Pets.Application.Commands
{
    public sealed record UpdatePetCommand(
        Guid Id,
        string Name,
        string Species,
        DateOnly? BirthDate);
}
