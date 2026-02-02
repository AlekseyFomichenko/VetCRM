using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed class DisableUserHandler(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(DisableUserCommand command, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, ct);
            if (user is null)
                throw new UserNotFoundException(command.UserId);

            user.Disable();
            await _userRepository.SaveAsync(ct);
        }
    }
}
