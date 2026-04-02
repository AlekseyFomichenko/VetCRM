using VetCRM.Modules.Clients.Domain;

namespace VetCRM.Modules.Clients.Application.Queries
{
    public sealed record GetClientByIdResult(
        Guid Id,
        string FullName,
        string Phone,
        string? Email,
        string? Address,
        string? Notes,
        ClientStatus Status,
        DateOnly CreatedAt);
}
