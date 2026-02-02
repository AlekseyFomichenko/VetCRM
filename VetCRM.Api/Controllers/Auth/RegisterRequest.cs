using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Api.Controllers.Auth
{
    public sealed record RegisterRequest(string Email, string Password, UserRole Role);
}
