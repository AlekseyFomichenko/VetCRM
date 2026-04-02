using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Queries
{
    public sealed record VeterinarianListItem(Guid Id, string Email, string? FullName);

    public sealed class GetVeterinariansForSchedulingHandler(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<IReadOnlyList<VeterinarianListItem>> Handle(CancellationToken ct)
        {
            (var items, _) = await _userRepository.GetListAsync(
                search: null,
                role: UserRole.Veterinarian,
                status: UserStatus.Active,
                page: 1,
                pageSize: 200,
                ct);

            return items
                .Select(u => new VeterinarianListItem(u.Id, u.Email, u.FullName))
                .ToList();
        }
    }
}
