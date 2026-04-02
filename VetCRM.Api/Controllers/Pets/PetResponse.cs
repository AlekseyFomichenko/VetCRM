using VetCRM.Modules.Pets.Domain;

namespace VetCRM.Api.Controllers.Pets
{
    public sealed record PetResponse(
        Guid Id,
        Guid? ClientId,
        string Name,
        string Species,
        DateOnly? BirthDate,
        PetStatus Status);
}
