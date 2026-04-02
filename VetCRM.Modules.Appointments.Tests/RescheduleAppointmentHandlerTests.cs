using Moq;
using VetCRM.Modules.Appointments.Application.Commands;
using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;
using VetCRM.SharedKernel;
using Xunit;

namespace VetCRM.Modules.Appointments.Tests
{
    public sealed class RescheduleAppointmentHandlerTests
    {
        [Fact]
        public async Task Handle_WhenAppointmentExistsAndNoOverlap_Reschedules()
        {
            var appointment = Appointment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), null, null);
            var repoMock = new Mock<IAppointmentRepository>();
            repoMock.Setup(r => r.GetByIdAsync(appointment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(appointment);
            repoMock.Setup(r => r.HasOverlappingForVetAsync(It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), appointment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var handler = new RescheduleAppointmentHandler(repoMock.Object);
            var newStartsAt = DateTime.UtcNow.AddDays(2);
            var newEndsAt = newStartsAt.AddHours(1);
            var command = new RescheduleAppointmentCommand(appointment.Id, newStartsAt, newEndsAt);

            await handler.Handle(command, CancellationToken.None);

            repoMock.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(newStartsAt, appointment.StartsAt);
            Assert.Equal(newEndsAt, appointment.EndsAt);
        }

        [Fact]
        public async Task Handle_WhenAppointmentNotFound_ThrowsAppointmentNotFoundException()
        {
            var repoMock = new Mock<IAppointmentRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Appointment?)null);

            var handler = new RescheduleAppointmentHandler(repoMock.Object);
            var command = new RescheduleAppointmentCommand(Guid.NewGuid(), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

            await Assert.ThrowsAsync<AppointmentNotFoundException>(() => handler.Handle(command, CancellationToken.None));
            repoMock.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenOverlapExists_ThrowsAppointmentConflictException()
        {
            var vetId = Guid.NewGuid();
            var appointment = Appointment.Create(Guid.NewGuid(), Guid.NewGuid(), vetId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), null, null);
            var repoMock = new Mock<IAppointmentRepository>();
            repoMock.Setup(r => r.GetByIdAsync(appointment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(appointment);
            repoMock.Setup(r => r.HasOverlappingForVetAsync(vetId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), appointment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var handler = new RescheduleAppointmentHandler(repoMock.Object);
            var command = new RescheduleAppointmentCommand(appointment.Id, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1));

            await Assert.ThrowsAsync<AppointmentConflictException>(() => handler.Handle(command, CancellationToken.None));
            repoMock.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
