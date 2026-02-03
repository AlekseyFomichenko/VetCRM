namespace VetCRM.Modules.Pets.Application.Queries
{
    public sealed record GetPetsResult(
        IReadOnlyList<GetPetByIdResult> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
