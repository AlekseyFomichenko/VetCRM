using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed class RegisterUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken ct)
        {
            if (command.Role == UserRole.Admin)
                throw new ArgumentException("Admin cannot self-register.");

            bool exists = await _userRepository.ExistsByEmailAsync(command.Email, null, ct);
            if (exists)
                throw new DuplicateEmailException(command.Email);

            string hash = _passwordHasher.Hash(command.Password);
            User user = User.Create(command.Email, hash, command.Role, null);
            await _userRepository.AddAsync(user, ct);
            return new RegisterUserResult(user.Id);
        }
    }
}
