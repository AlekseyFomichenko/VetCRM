using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed record CreateUserCommand(
        string Email,
        string Password,
        UserRole Role,
        string? FullName);
}
