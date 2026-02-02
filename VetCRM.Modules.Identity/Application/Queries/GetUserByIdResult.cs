using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Queries
{
    public sealed record GetUserByIdResult(
        Guid Id,
        string Email,
        UserRole Role,
        string? FullName,
        UserStatus Status,
        DateTime CreatedAt);
}
