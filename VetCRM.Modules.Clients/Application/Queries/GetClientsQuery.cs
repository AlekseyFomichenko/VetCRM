using VetCRM.Modules.Clients.Domain;

namespace VetCRM.Modules.Clients.Application.Queries
{
    public sealed record GetClientsQuery(
        string? Search,
        int Page,
        int PageSize,
        ClientStatus? Status);
}
