using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed class CreateUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken ct)
        {
            bool exists = await _userRepository.ExistsByEmailAsync(command.Email, null, ct);
            if (exists)
                throw new DuplicateEmailException(command.Email);

            string hash = _passwordHasher.Hash(command.Password);
            User user = User.Create(command.Email, hash, command.Role, command.FullName);
            await _userRepository.AddAsync(user, ct);
            return new CreateUserResult(user.Id);
        }
    }
}
