using Moq;
using VetCRM.Modules.Appointments.Application.Commands;
using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.SharedKernel;
using Xunit;

namespace VetCRM.Modules.Appointments.Tests
{
    public sealed class CreateAppointmentHandlerTests
    {
        [Fact]
        public async Task Handle_WhenAllChecksPass_CreatesAppointment()
        {
            var petId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var repoMock = new Mock<IAppointmentRepository>();
            var petMock = new Mock<IPetReadService>();
            var clientMock = new Mock<IClientReadService>();
            petMock.Setup(p => p.ExistsAsync(petId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            clientMock.Setup(c => c.ExistsAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            repoMock.Setup(r => r.HasOverlappingForVetAsync(It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var handler = new CreateAppointmentHandler(repoMock.Object, petMock.Object, clientMock.Object);
            var command = new CreateAppointmentCommand(petId, clientId, Guid.NewGuid(), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), "Checkup");

            var result = await handler.Handle(command, CancellationToken.None);

            repoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(Guid.Empty, result.AppointmentId);
        }

        [Fact]
        public async Task Handle_WhenPetNotFound_ThrowsPetNotFoundException()
        {
            var petId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var repoMock = new Mock<IAppointmentRepository>();
            var petMock = new Mock<IPetReadService>();
            var clientMock = new Mock<IClientReadService>();
            petMock.Setup(p => p.ExistsAsync(petId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var handler = new CreateAppointmentHandler(repoMock.Object, petMock.Object, clientMock.Object);
            var command = new CreateAppointmentCommand(petId, clientId, null, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), null);

            await Assert.ThrowsAsync<PetNotFoundException>(() => handler.Handle(command, CancellationToken.None));
            repoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenClientNotFound_ThrowsClientNotFoundException()
        {
            var petId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var repoMock = new Mock<IAppointmentRepository>();
            var petMock = new Mock<IPetReadService>();
            var clientMock = new Mock<IClientReadService>();
            petMock.Setup(p => p.ExistsAsync(petId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            clientMock.Setup(c => c.ExistsAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var handler = new CreateAppointmentHandler(repoMock.Object, petMock.Object, clientMock.Object);
            var command = new CreateAppointmentCommand(petId, clientId, null, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), null);

            await Assert.ThrowsAsync<ClientNotFoundException>(() => handler.Handle(command, CancellationToken.None));
            repoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenOverlapExists_ThrowsAppointmentConflictException()
        {
            var petId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var vetId = Guid.NewGuid();
            var repoMock = new Mock<IAppointmentRepository>();
            var petMock = new Mock<IPetReadService>();
            var clientMock = new Mock<IClientReadService>();
            petMock.Setup(p => p.ExistsAsync(petId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            clientMock.Setup(c => c.ExistsAsync(clientId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            repoMock.Setup(r => r.HasOverlappingForVetAsync(vetId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var handler = new CreateAppointmentHandler(repoMock.Object, petMock.Object, clientMock.Object);
            var startsAt = DateTime.UtcNow.AddHours(1);
            var endsAt = DateTime.UtcNow.AddHours(2);
            var command = new CreateAppointmentCommand(petId, clientId, vetId, startsAt, endsAt, null);

            var ex = await Assert.ThrowsAsync<AppointmentConflictException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal(vetId, ex.VeterinarianUserId);
            repoMock.Verify(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
