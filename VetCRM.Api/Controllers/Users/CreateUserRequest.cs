using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Api.Controllers.Users
{
    public sealed record CreateUserRequest(string Email, string Password, UserRole Role, string? FullName);
}
