using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCRM.Modules.Reports.Application.Queries;

namespace VetCRM.Api.Controllers.Reports
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "Admin")]
    public sealed class ReportsController(
        GetAppointmentsReportHandler getAppointmentsReportHandler,
        GetOverdueVaccinationsReportHandler getOverdueVaccinationsReportHandler) : Controller
    {
        private readonly GetAppointmentsReportHandler _getAppointmentsReportHandler = getAppointmentsReportHandler;
        private readonly GetOverdueVaccinationsReportHandler _getOverdueVaccinationsReportHandler = getOverdueVaccinationsReportHandler;

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentsReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = new GetAppointmentsReportQuery(from, to, page, pageSize);
            var result = await _getAppointmentsReportHandler.Handle(query, ct);

            var response = new AppointmentsReportResponse(
                result.TotalCount,
                result.Items.Select(i => new AppointmentReportItemResponse(
                    i.Id,
                    i.PetId,
                    i.ClientId,
                    i.VeterinarianUserId,
                    i.StartsAt,
                    i.EndsAt,
                    i.Status,
                    i.Reason,
                    i.CreatedAt)).ToList(),
                result.Page,
                result.PageSize);

            return Ok(response);
        }

        [HttpGet("overdue-vaccinations")]
        public async Task<IActionResult> GetOverdueVaccinationsReport(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = new GetOverdueVaccinationsReportQuery(page, pageSize);
            var result = await _getOverdueVaccinationsReportHandler.Handle(query, ct);

            var response = new OverdueVaccinationsReportResponse(
                result.TotalCount,
                result.Items.Select(i => new OverdueVaccinationItemResponse(
                    i.VaccinationId,
                    i.PetId,
                    i.VaccineName,
                    i.NextDueDate,
                    i.IsOverdue,
                    i.ClientFullName,
                    i.ClientPhone,
                    i.ClientEmail)).ToList(),
                result.Page,
                result.PageSize);

            return Ok(response);
        }
    }
}
