namespace VetCRM.Api.Controllers.Clients
{
    public sealed record GetClientsResponse(
        IReadOnlyList<ClientResponse> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
