using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCRM.Modules.Notifications.Application.Commands;
using VetCRM.Modules.Notifications.Application.Queries;
using VetCRM.Modules.Notifications.Domain;

namespace VetCRM.Api.Controllers.Notifications
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize(Roles = "Admin")]
    public sealed class NotificationsController(
        ProcessVaccinationRemindersHandler processRemindersHandler,
        GetReminderLogHandler getReminderLogHandler) : Controller
    {
        private readonly ProcessVaccinationRemindersHandler _processRemindersHandler = processRemindersHandler;
        private readonly GetReminderLogHandler _getReminderLogHandler = getReminderLogHandler;

        [HttpPost("send")]
        public async Task<IActionResult> Send(CancellationToken ct)
        {
            var result = await _processRemindersHandler.Handle(new ProcessVaccinationRemindersCommand(), ct);
            return Ok(new ProcessVaccinationRemindersResponse(result.Created, result.Sent, result.Failed));
        }

        [HttpGet("log")]
        public async Task<IActionResult> GetLog(
            [FromQuery] ReminderType? type,
            [FromQuery] ReminderStatus? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken ct = default)
        {
            var query = new GetReminderLogQuery(type, status, from, to);
            var result = await _getReminderLogHandler.Handle(query, ct);

            var response = result.Items.Select(r => new ReminderLogResponse(
                r.Id,
                r.Type,
                r.TargetClientId,
                r.TargetPetId,
                r.Channel,
                r.Payload,
                r.Status,
                r.CreatedAt,
                r.Error)).ToList();

            return Ok(response);
        }
    }
}
