using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Queries
{
    public sealed class GetUsersHandler(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<GetUsersResult> Handle(GetUsersQuery query, CancellationToken ct)
        {
            (var items, int totalCount) = await _userRepository.GetListAsync(
                query.Search,
                query.Role,
                query.Status,
                query.Page,
                query.PageSize,
                ct);

            var resultItems = items.Select(u => new GetUserByIdResult(
                u.Id,
                u.Email,
                u.Role,
                u.FullName,
                u.Status,
                u.CreatedAt)).ToList();

            return new GetUsersResult(
                resultItems,
                totalCount,
                query.Page,
                query.PageSize);
        }
    }
}
