using VetCRM.Modules.Clients.Domain;

namespace VetCRM.Api.Controllers.Clients
{
    public sealed record ClientResponse(
        Guid Id,
        string FullName,
        string Phone,
        string? Email,
        string? Address,
        string? Notes,
        ClientStatus Status,
        DateOnly CreatedAt);
}
