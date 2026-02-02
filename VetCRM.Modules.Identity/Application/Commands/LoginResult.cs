using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed record LoginResult(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        Guid UserId,
        string Email,
        UserRole Role);
}
