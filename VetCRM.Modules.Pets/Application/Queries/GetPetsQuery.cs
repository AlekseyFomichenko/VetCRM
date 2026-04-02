using VetCRM.Modules.Pets.Domain;

namespace VetCRM.Modules.Pets.Application.Queries
{
    public sealed record GetPetsQuery(
        string? Search,
        int Page,
        int PageSize,
        Guid? ClientId,
        PetStatus? Status);
}
