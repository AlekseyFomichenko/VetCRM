using Microsoft.AspNetCore.Mvc;
using VetCRM.Modules.Identity.Application.Commands;

namespace VetCRM.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController(
        RegisterUserHandler registerHandler,
        LoginHandler loginHandler,
        RefreshTokenHandler refreshTokenHandler,
        ForgotPasswordHandler forgotPasswordHandler,
        ResetPasswordHandler resetPasswordHandler) : Controller
    {
        private readonly RegisterUserHandler _registerHandler = registerHandler;
        private readonly LoginHandler _loginHandler = loginHandler;
        private readonly RefreshTokenHandler _refreshTokenHandler = refreshTokenHandler;
        private readonly ForgotPasswordHandler _forgotPasswordHandler = forgotPasswordHandler;
        private readonly ResetPasswordHandler _resetPasswordHandler = resetPasswordHandler;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.Password,
                request.Role,
                request.FullName);
            var result = await _registerHandler.Handle(command, ct);
            return Created($"/api/users/{result.UserId}", new { userId = result.UserId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await _loginHandler.Handle(command, ct);
            return Ok(new LoginResponse(
                result.AccessToken,
                result.RefreshToken,
                result.ExpiresAt,
                result.UserId,
                result.Email,
                result.Role.ToString()));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await _refreshTokenHandler.Handle(command, ct);
            return Ok(new LoginResponse(
                result.AccessToken,
                result.RefreshToken,
                result.ExpiresAt,
                result.UserId,
                result.Email,
                result.Role.ToString()));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
        {
            var command = new ForgotPasswordCommand(request.Email);
            await _forgotPasswordHandler.Handle(command, ct);
            return NoContent();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
        {
            var command = new ResetPasswordCommand(request.Token, request.NewPassword);
            await _resetPasswordHandler.Handle(command, ct);
            return NoContent();
        }
    }
}
