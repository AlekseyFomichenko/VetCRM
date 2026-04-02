using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Identity.Application.Commands
{
    public sealed class LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenStore refreshTokenStore)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IRefreshTokenStore _refreshTokenStore = refreshTokenStore;

        public async Task<LoginResult> Handle(LoginCommand command, CancellationToken ct)
        {
            User? user = await _userRepository.GetByEmailAsync(command.Email, ct);
            if (user is null)
                throw new InvalidCredentialsException();

            if (user.Status == UserStatus.Disabled)
                throw new AccountDisabledException();

            if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
                throw new InvalidCredentialsException();

            string accessToken = _tokenService.GenerateAccessToken(user);
            (string refreshToken, DateTime expiresAt) = _tokenService.GenerateRefreshToken();
            await _refreshTokenStore.AddAsync(user.Id, refreshToken, expiresAt, ct);

            return new LoginResult(
                accessToken,
                refreshToken,
                expiresAt,
                user.Id,
                user.Email,
                user.Role);
        }
    }
}
