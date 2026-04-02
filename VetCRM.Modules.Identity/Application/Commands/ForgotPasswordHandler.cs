using VetCRM.Modules.Identity.Application.Contracts;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed class ForgotPasswordHandler(
        IUserRepository userRepository,
        IPasswordResetTokenStore resetTokenStore,
        IEmailSender emailSender)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordResetTokenStore _resetTokenStore = resetTokenStore;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task Handle(ForgotPasswordCommand command, CancellationToken ct)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email, ct);
            if (user is null)
                return;

            string token = Guid.NewGuid().ToString("N");
            DateTime expiresAt = DateTime.UtcNow.AddHours(1);
            await _resetTokenStore.AddAsync(user.Id, token, expiresAt, ct);
            await _emailSender.SendPasswordResetAsync(command.Email, token, ct);
        }
    }
}
