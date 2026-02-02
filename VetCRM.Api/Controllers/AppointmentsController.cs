using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCRM.Api.Controllers.Appointments;
using VetCRM.Modules.Appointments.Application.Commands;
using VetCRM.Modules.Appointments.Application.Queries;

namespace VetCRM.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
    public sealed class AppointmentsController(
        CreateAppointmentHandler createHandler,
        RescheduleAppointmentHandler rescheduleHandler,
        CancelAppointmentHandler cancelHandler,
        CompleteAppointmentHandler completeHandler,
        GetAppointmentsByDateHandler getByDateHandler) : Controller
    {
        private readonly CreateAppointmentHandler _createHandler = createHandler;
        private readonly RescheduleAppointmentHandler _rescheduleHandler = rescheduleHandler;
        private readonly CancelAppointmentHandler _cancelHandler = cancelHandler;
        private readonly CompleteAppointmentHandler _completeHandler = completeHandler;
        private readonly GetAppointmentsByDateHandler _getByDateHandler = getByDateHandler;

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request, CancellationToken ct)
        {
            var command = new CreateAppointmentCommand(
                request.PetId,
                request.ClientId,
                request.VeterinarianUserId,
                request.StartsAt,
                request.EndsAt,
                request.Reason);

            var result = await _createHandler.Handle(command, ct);
            return CreatedAtAction(nameof(GetByDate), new { date = request.StartsAt.Date.ToString("yyyy-MM-dd") }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetByDate(
            [FromQuery] string date,
            [FromQuery] Guid? vetId,
            CancellationToken ct = default)
        {
            if (!DateOnly.TryParse(date, out var dateOnly))
                return BadRequest("Invalid date format. Use YYYY-MM-DD.");

            var query = new GetAppointmentsByDateQuery(dateOnly, vetId);
            var appointments = await _getByDateHandler.Handle(query, ct);
            var response = appointments.Select(a => new AppointmentResponse(
                a.Id,
                a.PetId,
                a.ClientId,
                a.VeterinarianUserId,
                a.StartsAt,
                a.EndsAt,
                a.Status,
                a.Reason,
                a.CreatedByUserId,
                a.CreatedAt)).ToList();
            return Ok(response);
        }

        [HttpPut("{id:guid}/reschedule")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentRequest request, CancellationToken ct)
        {
            await _rescheduleHandler.Handle(new RescheduleAppointmentCommand(id, request.StartsAt, request.EndsAt), ct);
            return Ok();
        }

        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            await _cancelHandler.Handle(new CancelAppointmentCommand(id), ct);
            return Ok();
        }

        [HttpPut("{id:guid}/complete")]
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteAppointmentRequest request, CancellationToken ct)
        {
            await _completeHandler.Handle(new CompleteAppointmentCommand(
                id,
                request.Complaint,
                request.Diagnosis,
                request.TreatmentPlan,
                request.Prescription,
                request.Attachments), ct);
            return Ok();
        }
    }
}
