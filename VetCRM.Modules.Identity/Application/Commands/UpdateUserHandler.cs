using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed class UpdateUserHandler(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(UpdateUserCommand command, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, ct);
            if (user is null)
                throw new UserNotFoundException(command.UserId);

            user.Update(command.FullName, command.Role);
            await _userRepository.SaveAsync(ct);
        }
    }
}
