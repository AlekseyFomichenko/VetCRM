using Moq;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Pets.Application.Commands;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.SharedKernel;
using Xunit;

namespace VetCRM.Modules.Pets.Tests
{
    public sealed class CreatePetHandlerTests
    {
        [Fact]
        public async Task Handle_WhenClientIdIsNull_CreatesPet_DoesNotCallClientReadService()
        {
            var petsMock = new Mock<IPetRepository>();
            var clientsMock = new Mock<IClientReadService>();

            var handler = new CreatePetHandler(petsMock.Object, clientsMock.Object);
            var command = new CreatePetCommand(
                ClientId: null,
                Name: "Rex",
                Species: "Dog",
                BirthDate: new DateOnly(2020, 5, 1));

            var result = await handler.Handle(command, CancellationToken.None);

            clientsMock.Verify(c => c.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            petsMock.Verify(p => p.AddAsync(It.IsAny<Domain.Pet>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(Guid.Empty, result.PetId);
        }

        [Fact]
        public async Task Handle_WhenClientIdProvidedAndExists_CreatesPet()
        {
            var clientId = Guid.NewGuid();
            var petsMock = new Mock<IPetRepository>();
            var clientsMock = new Mock<IClientReadService>();
            clientsMock.Setup(c => c.ExistsAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var handler = new CreatePetHandler(petsMock.Object, clientsMock.Object);
            var command = new CreatePetCommand(
                ClientId: clientId,
                Name: "Rex",
                Species: "Dog",
                BirthDate: new DateOnly(2020, 5, 1));

            var result = await handler.Handle(command, CancellationToken.None);

            clientsMock.Verify(c => c.ExistsAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
            petsMock.Verify(p => p.AddAsync(It.IsAny<Domain.Pet>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(Guid.Empty, result.PetId);
        }

        [Fact]
        public async Task Handle_WhenClientIdProvidedAndNotExists_ThrowsClientNotFoundException()
        {
            var clientId = Guid.NewGuid();
            var petsMock = new Mock<IPetRepository>();
            var clientsMock = new Mock<IClientReadService>();
            clientsMock.Setup(c => c.ExistsAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var handler = new CreatePetHandler(petsMock.Object, clientsMock.Object);
            var command = new CreatePetCommand(
                ClientId: clientId,
                Name: "Rex",
                Species: "Dog",
                BirthDate: null);

            await Assert.ThrowsAsync<ClientNotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenClientNotFound_DoesNotCallAddAsync()
        {
            var clientId = Guid.NewGuid();
            var petsMock = new Mock<IPetRepository>();
            var clientsMock = new Mock<IClientReadService>();
            clientsMock.Setup(c => c.ExistsAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var handler = new CreatePetHandler(petsMock.Object, clientsMock.Object);
            var command = new CreatePetCommand(
                ClientId: clientId,
                Name: "Rex",
                Species: "Dog",
                BirthDate: null);

            try
            {
                await handler.Handle(command, CancellationToken.None);
            }
            catch (ClientNotFoundException)
            {
            }

            petsMock.Verify(p => p.AddAsync(It.IsAny<Domain.Pet>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
