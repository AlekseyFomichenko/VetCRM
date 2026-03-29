using Moq;
using VetCRM.Modules.Identity.Application.Commands;
using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;
using VetCRM.SharedKernel;
using Xunit;

namespace VetCRM.Modules.Identity.Tests
{
    public sealed class RegisterUserHandlerTests
    {
        [Fact]
        public async Task Handle_WhenEmailIsUnique_CreatesUser()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.ExistsByEmailAsync("user@example.com", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var passwordHasherMock = new Mock<IPasswordHasher>();
            passwordHasherMock.Setup(h => h.Hash("password123")).Returns("hashed");

            var handler = new RegisterUserHandler(userRepositoryMock.Object, passwordHasherMock.Object);
            var command = new RegisterUserCommand("user@example.com", "password123", UserRole.Receptionist, "Full Name");

            var result = await handler.Handle(command, CancellationToken.None);

            userRepositoryMock.Verify(r => r.ExistsByEmailAsync("user@example.com", null, It.IsAny<CancellationToken>()), Times.Once);
            userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(Guid.Empty, result.UserId);
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyExists_ThrowsDuplicateEmailException()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.ExistsByEmailAsync("user@example.com", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var passwordHasherMock = new Mock<IPasswordHasher>();

            var handler = new RegisterUserHandler(userRepositoryMock.Object, passwordHasherMock.Object);
            var command = new RegisterUserCommand("user@example.com", "password123", UserRole.Veterinarian, null);

            var ex = await Assert.ThrowsAsync<DuplicateEmailException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Equal("user@example.com", ex.Email);
            userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRoleIsAdmin_ThrowsArgumentException()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();

            var handler = new RegisterUserHandler(userRepositoryMock.Object, passwordHasherMock.Object);
            var command = new RegisterUserCommand("admin@example.com", "password123", UserRole.Admin, null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(command, CancellationToken.None));

            userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
