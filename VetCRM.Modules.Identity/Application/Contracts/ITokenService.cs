using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Application.Contracts
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        (string Token, DateTime ExpiresAt) GenerateRefreshToken();
        Task<Guid?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken);
    }
}
