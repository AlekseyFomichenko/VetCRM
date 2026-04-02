namespace VetCRM.Modules.Identity.Application.Contracts
{
    public interface IEmailSender
    {
        Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken);
    }
}
