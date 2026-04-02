using Moq;
using VetCRM.Modules.Clients.Application.Commands;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.SharedKernel;
using Xunit;

namespace VetCRM.Modules.Clients.Tests
{
    public sealed class CreateClientHandlerTests
    {
        [Fact]
        public async Task Handle_WhenPhoneIsUnique_CreatesClient()
        {
            var repositoryMock = new Mock<IClientRepository>();
            repositoryMock.Setup(r => r.ExistsByPhoneAsync("+79991234567", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new CreateClientHandler(repositoryMock.Object);
            var command = new CreateClientCommand(
                FullName: "Ivan Ivanov",
                Phone: "+79991234567",
                Email: "ivan@example.com",
                Address: null,
                Notes: null);

            var result = await handler.Handle(command, CancellationToken.None);

            repositoryMock.Verify(r => r.ExistsByPhoneAsync("+79991234567", null, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<VetCRM.Modules.Clients.Domain.Client>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(Guid.Empty, result.ClientId);
        }

        [Fact]
        public async Task Handle_WhenPhoneAlreadyExists_ThrowsDuplicatePhoneException()
        {
            var repositoryMock = new Mock<IClientRepository>();
            repositoryMock.Setup(r => r.ExistsByPhoneAsync("+79991234567", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = new CreateClientHandler(repositoryMock.Object);
            var command = new CreateClientCommand(
                FullName: "Ivan Ivanov",
                Phone: "+79991234567",
                Email: null,
                Address: null,
                Notes: null);

            var ex = await Assert.ThrowsAsync<DuplicatePhoneException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Equal("+79991234567", ex.Phone);
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<VetCRM.Modules.Clients.Domain.Client>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
