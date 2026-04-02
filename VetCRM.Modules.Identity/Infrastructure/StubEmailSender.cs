using Microsoft.Extensions.Logging;
using VetCRM.Modules.Identity.Application.Contracts;

namespace VetCRM.Modules.Identity.Infrastructure
{
    public sealed class StubEmailSender(ILogger<StubEmailSender> logger) : IEmailSender
    {
        private readonly ILogger<StubEmailSender> _logger = logger;

        public Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Stub: password reset email would be sent to {Email} with token {Token}",
                email,
                resetToken);
            return Task.CompletedTask;
        }
    }
}
