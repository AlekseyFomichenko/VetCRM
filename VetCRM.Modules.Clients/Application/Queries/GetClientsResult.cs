namespace VetCRM.Modules.Clients.Application.Queries
{
    public sealed record GetClientsResult(
        IReadOnlyList<GetClientByIdResult> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
