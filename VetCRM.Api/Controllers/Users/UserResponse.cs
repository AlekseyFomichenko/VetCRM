namespace VetCRM.Api.Controllers.Users
{
    public sealed record UserResponse(
        Guid Id,
        string Email,
        string Role,
        string? FullName,
        string Status,
        DateTime CreatedAt);
}
