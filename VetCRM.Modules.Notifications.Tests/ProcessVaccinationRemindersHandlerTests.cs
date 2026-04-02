using Moq;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.Notifications.Application.Commands;
using VetCRM.Modules.Notifications.Application.Contracts;
using VetCRM.Modules.Notifications.Domain;
using VetCRM.Modules.Pets.Application.Contracts;
using Xunit;

namespace VetCRM.Modules.Notifications.Tests
{
    public sealed class ProcessVaccinationRemindersHandlerTests
    {
        [Fact]
        public async Task Handle_WhenUpcomingVaccinationsExist_AddsReminderLogsAndReturnsResult()
        {
            var petId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var upcomingDto = new UpcomingVaccinationDto(
                Guid.NewGuid(),
                petId,
                Guid.NewGuid(),
                "Rabies",
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2));

            var upcomingQueryMock = new Mock<IUpcomingVaccinationsQuery>();
            upcomingQueryMock.Setup(q => q.GetUpcomingAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { upcomingDto });

            var petReadServiceMock = new Mock<IPetReadService>();
            petReadServiceMock.Setup(p => p.GetOwnerClientId(petId)).Returns(clientId);

            var clientReadServiceMock = new Mock<IClientReadService>();
            clientReadServiceMock.Setup(c => c.GetContactAsync(clientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClientContactDto("Ivan Ivanov", "+79991234567", "client@example.com"));

            var reminderLogRepositoryMock = new Mock<IReminderLogRepository>();

            var senderMock = new Mock<INotificationSender>();
            senderMock.Setup(s => s.Channel).Returns(ReminderChannel.Demo);
            senderMock.Setup(s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new ProcessVaccinationRemindersHandler(
                upcomingQueryMock.Object,
                petReadServiceMock.Object,
                clientReadServiceMock.Object,
                reminderLogRepositoryMock.Object,
                new[] { senderMock.Object });

            var result = await handler.Handle(new ProcessVaccinationRemindersCommand(), CancellationToken.None);

            Assert.Equal(1, result.Created);
            Assert.Equal(1, result.Sent);
            Assert.Equal(0, result.Failed);
            reminderLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ReminderLog>(), It.IsAny<CancellationToken>()), Times.Once);
            reminderLogRepositoryMock.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenPetHasNoOwner_SkipsReminder()
        {
            var petId = Guid.NewGuid();
            var upcomingDto = new UpcomingVaccinationDto(
                Guid.NewGuid(),
                petId,
                Guid.NewGuid(),
                "Rabies",
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2));

            var upcomingQueryMock = new Mock<IUpcomingVaccinationsQuery>();
            upcomingQueryMock.Setup(q => q.GetUpcomingAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { upcomingDto });

            var petReadServiceMock = new Mock<IPetReadService>();
            petReadServiceMock.Setup(p => p.GetOwnerClientId(petId)).Returns(Guid.Empty);

            var clientReadServiceMock = new Mock<IClientReadService>();
            var reminderLogRepositoryMock = new Mock<IReminderLogRepository>();

            var handler = new ProcessVaccinationRemindersHandler(
                upcomingQueryMock.Object,
                petReadServiceMock.Object,
                clientReadServiceMock.Object,
                reminderLogRepositoryMock.Object,
                Array.Empty<INotificationSender>());

            var result = await handler.Handle(new ProcessVaccinationRemindersCommand(), CancellationToken.None);

            Assert.Equal(0, result.Created);
            Assert.Equal(0, result.Sent);
            reminderLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ReminderLog>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
