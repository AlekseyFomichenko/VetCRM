using Moq;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.Modules.Reports.Application.Queries;
using Xunit;

namespace VetCRM.Modules.Reports.Tests
{
    public sealed class GetOverdueVaccinationsReportHandlerTests
    {
        [Fact]
        public async Task Handle_WhenVaccinationsExist_ReturnsPagedResultWithClientContact()
        {
            var petId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var vaccinationDto = new UpcomingVaccinationDto(
                Guid.NewGuid(),
                petId,
                Guid.NewGuid(),
                "Rabies",
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1));

            var upcomingQueryMock = new Mock<IUpcomingVaccinationsQuery>();
            upcomingQueryMock.Setup(q => q.GetUpcomingAsync(
                    It.IsAny<DateOnly>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { vaccinationDto });

            var petReadServiceMock = new Mock<IPetReadService>();
            petReadServiceMock.Setup(p => p.GetOwnerClientId(petId)).Returns(clientId);

            var clientReadServiceMock = new Mock<IClientReadService>();
            clientReadServiceMock.Setup(c => c.GetContactAsync(clientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClientContactDto("Ivan Ivanov", "+79991234567", "ivan@example.com"));

            var handler = new GetOverdueVaccinationsReportHandler(
                upcomingQueryMock.Object,
                petReadServiceMock.Object,
                clientReadServiceMock.Object);

            var query = new GetOverdueVaccinationsReportQuery(1, 20);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal(vaccinationDto.VaccinationId, result.Items[0].VaccinationId);
            Assert.True(result.Items[0].IsOverdue);
            Assert.Equal("Ivan Ivanov", result.Items[0].ClientFullName);
            Assert.Equal("+79991234567", result.Items[0].ClientPhone);
        }
    }
}
