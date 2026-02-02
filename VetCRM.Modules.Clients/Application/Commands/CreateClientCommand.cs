namespace VetCRM.Modules.Clients.Application.Commands
{
    public sealed record CreateClientCommand(
        string FullName,
        string Phone,
        string? Email,
        string? Address,
        string? Notes);
}
