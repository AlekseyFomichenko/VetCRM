using Moq;
using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Reports.Application.Queries;
using VetCRM.SharedKernel;
using Xunit;

namespace VetCRM.Modules.Reports.Tests
{
    public sealed class GetAppointmentsReportHandlerTests
    {
        [Fact]
        public async Task Handle_WhenPeriodValid_ReturnsResult()
        {
            var from = new DateTime(2025, 1, 1);
            var to = new DateTime(2025, 1, 31);
            var dto = new AppointmentReportDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                from.AddHours(9),
                from.AddHours(10),
                1,
                null,
                from);

            var queryMock = new Mock<IAppointmentsForReportQuery>();
            queryMock.Setup(q => q.GetByDateRangeAsync(
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    1,
                    20,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((new[] { dto }, 1));

            var handler = new GetAppointmentsReportHandler(queryMock.Object);
            var query = new GetAppointmentsReportQuery(from, to, 1, 20);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal(dto.Id, result.Items[0].Id);
        }

        [Fact]
        public async Task Handle_WhenPeriodLongerThanOneYear_ThrowsReportPeriodTooLongException()
        {
            var from = new DateTime(2025, 1, 1);
            var to = new DateTime(2026, 2, 1);

            var queryMock = new Mock<IAppointmentsForReportQuery>();
            var handler = new GetAppointmentsReportHandler(queryMock.Object);
            var query = new GetAppointmentsReportQuery(from, to, 1, 20);

            await Assert.ThrowsAsync<ReportPeriodTooLongException>(() =>
                handler.Handle(query, CancellationToken.None));

            queryMock.Verify(q => q.GetByDateRangeAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
