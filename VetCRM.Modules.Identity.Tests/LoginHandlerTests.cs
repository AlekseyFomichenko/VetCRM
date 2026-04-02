using Moq;
using VetCRM.Modules.Identity.Application.Commands;
using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;
using VetCRM.SharedKernel;
using Xunit;

namespace VetCRM.Modules.Identity.Tests
{
    public sealed class LoginHandlerTests
    {
        [Fact]
        public async Task Handle_WhenCredentialsValid_ReturnsLoginResult()
        {
            var user = User.Create("user@example.com", "hashed", UserRole.Receptionist, null);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var passwordHasherMock = new Mock<IPasswordHasher>();
            passwordHasherMock.Setup(h => h.Verify("password123", "hashed")).Returns(true);

            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).Returns("access-token");
            tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

            var refreshTokenStoreMock = new Mock<IRefreshTokenStore>();

            var handler = new LoginHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                tokenServiceMock.Object,
                refreshTokenStoreMock.Object);

            var command = new LoginCommand("user@example.com", "password123");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal("access-token", result.AccessToken);
            Assert.Equal("refresh-token", result.RefreshToken);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal("user@example.com", result.Email);
            Assert.Equal(UserRole.Receptionist, result.Role);
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ThrowsInvalidCredentialsException()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmailAsync("unknown@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var passwordHasherMock = new Mock<IPasswordHasher>();
            var tokenServiceMock = new Mock<ITokenService>();
            var refreshTokenStoreMock = new Mock<IRefreshTokenStore>();

            var handler = new LoginHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                tokenServiceMock.Object,
                refreshTokenStoreMock.Object);

            var command = new LoginCommand("unknown@example.com", "password123");

            await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenUserDisabled_ThrowsAccountDisabledException()
        {
            var user = User.Create("user@example.com", "hashed", UserRole.Receptionist, null);
            user.Disable();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var passwordHasherMock = new Mock<IPasswordHasher>();
            passwordHasherMock.Setup(h => h.Verify("password123", "hashed")).Returns(true);

            var tokenServiceMock = new Mock<ITokenService>();
            var refreshTokenStoreMock = new Mock<IRefreshTokenStore>();

            var handler = new LoginHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                tokenServiceMock.Object,
                refreshTokenStoreMock.Object);

            var command = new LoginCommand("user@example.com", "password123");

            await Assert.ThrowsAsync<AccountDisabledException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenPasswordInvalid_ThrowsInvalidCredentialsException()
        {
            var user = User.Create("user@example.com", "hashed", UserRole.Receptionist, null);

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var passwordHasherMock = new Mock<IPasswordHasher>();
            passwordHasherMock.Setup(h => h.Verify("wrongpassword", "hashed")).Returns(false);

            var tokenServiceMock = new Mock<ITokenService>();
            var refreshTokenStoreMock = new Mock<IRefreshTokenStore>();

            var handler = new LoginHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                tokenServiceMock.Object,
                refreshTokenStoreMock.Object);

            var command = new LoginCommand("user@example.com", "wrongpassword");

            await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}
