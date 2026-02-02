namespace VetCRM.Api.Controllers.Clients
{
    public sealed record CreateClientRequest(
        string FullName,
        string Phone,
        string? Email,
        string? Address,
        string? Notes);
}
