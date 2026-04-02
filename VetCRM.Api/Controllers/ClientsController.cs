using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCRM.Api.Controllers.Clients;
using VetCRM.Modules.Clients.Application.Commands;
using VetCRM.Modules.Clients.Application.Queries;
using VetCRM.Modules.Clients.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Api.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
    public sealed class ClientsController(
        CreateClientHandler createHandler,
        UpdateClientHandler updateHandler,
        ArchiveClientHandler archiveHandler,
        GetClientByIdHandler getByIdHandler,
        GetClientsHandler getClientsHandler) : Controller
    {
        private readonly CreateClientHandler _createHandler = createHandler;
        private readonly UpdateClientHandler _updateHandler = updateHandler;
        private readonly ArchiveClientHandler _archiveHandler = archiveHandler;
        private readonly GetClientByIdHandler _getByIdHandler = getByIdHandler;
        private readonly GetClientsHandler _getClientsHandler = getClientsHandler;

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create([FromBody] CreateClientRequest request, CancellationToken ct)
        {
            var command = new CreateClientCommand(
                request.FullName,
                request.Phone,
                request.Email,
                request.Address,
                request.Notes);

            var result = await _createHandler.Handle(command, ct);

            return CreatedAtAction(nameof(GetById), new { id = result.ClientId }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] ClientStatus? status = null,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = new GetClientsQuery(search, page, pageSize, status);
            var result = await _getClientsHandler.Handle(query, ct);

            var response = new GetClientsResponse(
                result.Items.Select(Map).ToList(),
                result.TotalCount,
                result.Page,
                result.PageSize);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _getByIdHandler.Handle(new GetClientByIdQuery(id), ct);
            if (result is null)
                throw new ClientNotFoundException(id);

            return Ok(Map(result));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientRequest request, CancellationToken ct)
        {
            var command = new UpdateClientCommand(
                id,
                request.FullName,
                request.Phone,
                request.Email,
                request.Address,
                request.Notes);

            await _updateHandler.Handle(command, ct);
            return Ok();
        }

        [HttpPost("{id:guid}/archive")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
        {
            await _archiveHandler.Handle(new ArchiveClientCommand(id), ct);
            return Ok();
        }

        private static ClientResponse Map(GetClientByIdResult r) =>
            new(r.Id, r.FullName, r.Phone, r.Email, r.Address, r.Notes, r.Status, r.CreatedAt);
    }
}
