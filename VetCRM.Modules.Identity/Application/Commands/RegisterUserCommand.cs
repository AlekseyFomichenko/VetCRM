using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed record RegisterUserCommand(
        string Email,
        string Password,
        UserRole Role);
}
