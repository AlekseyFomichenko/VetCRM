namespace VetCRM.Api.Controllers.Pets
{
    public sealed record GetPetsResponse(
        IReadOnlyList<PetResponse> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
