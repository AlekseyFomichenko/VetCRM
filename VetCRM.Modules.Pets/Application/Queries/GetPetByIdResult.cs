using VetCRM.Modules.Pets.Domain;

namespace VetCRM.Modules.Pets.Application.Queries
{
    public sealed record GetPetByIdResult(
        Guid Id,
        Guid? ClientId,
        string Name,
        string Species,
        DateOnly? BirthDate,
        PetStatus Status);
}
