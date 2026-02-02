using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed class ResetPasswordHandler(
        IPasswordResetTokenStore resetTokenStore,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        private readonly IPasswordResetTokenStore _resetTokenStore = resetTokenStore;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task Handle(ResetPasswordCommand command, CancellationToken ct)
        {
            Guid? userId = await _resetTokenStore.GetUserIdByTokenAsync(command.Token, ct);
            if (userId is null)
                throw new ArgumentException("Invalid or expired reset token.");

            var user = await _userRepository.GetByIdAsync(userId.Value, ct);
            if (user is null)
                throw new ArgumentException("Invalid or expired reset token.");

            string hash = _passwordHasher.Hash(command.NewPassword);
            user.SetPassword(hash);
            await _resetTokenStore.ConsumeAsync(command.Token, ct);
            await _userRepository.SaveAsync(ct);
        }
    }
}
