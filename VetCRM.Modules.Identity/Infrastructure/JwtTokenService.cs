using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;

namespace VetCRM.Modules.Identity.Infrastructure
{
    public sealed class JwtTokenService(
        IOptions<JwtSettings> settings,
        IRefreshTokenStore refreshTokenStore) : ITokenService
    {
        private readonly JwtSettings _settings = settings.Value;
        private readonly IRefreshTokenStore _refreshTokenStore = refreshTokenStore;

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
        {
            string token = Guid.NewGuid().ToString("N");
            DateTime expiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
            return (token, expiresAt);
        }

        public async Task<Guid?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _refreshTokenStore.GetUserIdByTokenAsync(token, cancellationToken);
        }
    }
}
