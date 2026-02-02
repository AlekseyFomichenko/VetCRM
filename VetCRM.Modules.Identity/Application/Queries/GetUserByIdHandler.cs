using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Identity.Application.Queries
{
    public sealed class GetUserByIdHandler(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<GetUserByIdResult?> Handle(GetUserByIdQuery query, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId, ct);
            if (user is null)
                return null;

            return new GetUserByIdResult(
                user.Id,
                user.Email,
                user.Role,
                user.FullName,
                user.Status,
                user.CreatedAt);
        }
    }
}
