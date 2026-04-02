using Moq;
using VetCRM.Modules.MedicalRecords.Application.Commands;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using Xunit;

namespace VetCRM.Modules.MedicalRecords.Tests
{
    public sealed class CreateMedicalRecordFromAppointmentHandlerTests
    {
        [Fact]
        public async Task CreateAsync_CreatesMedicalRecordAndReturnsId()
        {
            var repoMock = new Mock<IMedicalRecordRepository>();
            var handler = new CreateMedicalRecordFromAppointmentHandler(repoMock.Object);
            var appointmentId = Guid.NewGuid();
            var petId = Guid.NewGuid();
            var vetId = Guid.NewGuid();

            var result = await handler.CreateAsync(
                appointmentId,
                petId,
                vetId,
                "Complaint",
                "Diagnosis",
                "Treatment",
                "Prescription",
                null,
                CancellationToken.None);

            repoMock.Verify(r => r.AddAsync(It.IsAny<Domain.MedicalRecord>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(Guid.Empty, result);
        }
    }
}
