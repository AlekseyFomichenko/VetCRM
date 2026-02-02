using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed class RefreshTokenHandler(
        ITokenService tokenService,
        IUserRepository userRepository,
        IRefreshTokenStore refreshTokenStore)
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRefreshTokenStore _refreshTokenStore = refreshTokenStore;

        public async Task<LoginResult> Handle(RefreshTokenCommand command, CancellationToken ct)
        {
            Guid? userId = await _tokenService.ValidateRefreshTokenAsync(command.RefreshToken, ct);
            if (userId is null)
                throw new InvalidCredentialsException();

            User? user = await _userRepository.GetByIdAsync(userId.Value, ct);
            if (user is null)
                throw new InvalidCredentialsException();

            if (user.Status == UserStatus.Disabled)
                throw new AccountDisabledException();

            await _refreshTokenStore.RevokeAsync(command.RefreshToken, ct);

            string accessToken = _tokenService.GenerateAccessToken(user);
            (string newRefreshToken, DateTime expiresAt) = _tokenService.GenerateRefreshToken();
            await _refreshTokenStore.AddAsync(user.Id, newRefreshToken, expiresAt, ct);

            return new LoginResult(
                accessToken,
                newRefreshToken,
                expiresAt,
                user.Id,
                user.Email,
                user.Role);
        }
    }
}
