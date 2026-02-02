using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Queries
{
    public sealed record GetUsersQuery(
        string? Search,
        UserRole? Role,
        UserStatus? Status,
        int Page,
        int PageSize);
}
