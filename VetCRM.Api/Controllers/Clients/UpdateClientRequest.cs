namespace VetCRM.Api.Controllers.Clients
{
    public sealed record UpdateClientRequest(
        string FullName,
        string Phone,
        string? Email,
        string? Address,
        string? Notes);
}
