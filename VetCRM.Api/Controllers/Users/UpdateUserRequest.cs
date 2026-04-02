using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Api.Controllers.Users
{
    public sealed record UpdateUserRequest(string? FullName, UserRole? Role);
}
