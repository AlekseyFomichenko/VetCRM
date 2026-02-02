namespace VetCRM.Modules.Identity.Application.Queries
{
    public sealed record GetUsersResult(
        IReadOnlyList<GetUserByIdResult> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
