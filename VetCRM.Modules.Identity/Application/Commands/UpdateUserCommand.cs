using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed record UpdateUserCommand(
        Guid UserId,
        string? FullName,
        UserRole? Role);
}
