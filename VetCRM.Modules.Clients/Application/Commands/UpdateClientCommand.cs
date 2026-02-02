namespace VetCRM.Modules.Clients.Application.Commands
{
    public sealed record UpdateClientCommand(
        Guid Id,
        string FullName,
        string Phone,
        string? Email,
        string? Address,
        string? Notes);
}
