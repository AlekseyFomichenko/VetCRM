using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCRM.Api.Controllers.Pets;
using VetCRM.Modules.Pets.Application.Commands;
using VetCRM.Modules.Pets.Application.Queries;
using VetCRM.Modules.Pets.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Api.Controllers
{
    [ApiController]
    [Route("api/pets")]
    [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
    public sealed class PetsController(
        CreatePetHandler createHandler,
        GetPetByIdHandler getByIdHandler,
        GetPetsHandler getPetsHandler,
        UpdatePetHandler updateHandler,
        ArchivePetHandler archiveHandler,
        SetPetClientHandler setClientHandler) : Controller
    {
        private readonly CreatePetHandler _createHandler = createHandler;
        private readonly GetPetByIdHandler _getByIdHandler = getByIdHandler;
        private readonly GetPetsHandler _getPetsHandler = getPetsHandler;
        private readonly UpdatePetHandler _updateHandler = updateHandler;
        private readonly ArchivePetHandler _archiveHandler = archiveHandler;
        private readonly SetPetClientHandler _setClientHandler = setClientHandler;

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create([FromBody] CreatePetRequest request, CancellationToken ct)
        {
            var command = new CreatePetCommand(
                Name: request.Name,
                Species: request.Species,
                BirthDate: request.BirthDate,
                ClientId: request.ClientId);

            var result = await _createHandler.Handle(command, ct);

            return CreatedAtAction(nameof(GetById), new { id = result.PetId }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? clientId = null,
            [FromQuery] PetStatus? status = null,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = new GetPetsQuery(search, page, pageSize, clientId, status);
            var result = await _getPetsHandler.Handle(query, ct);

            var response = new GetPetsResponse(
                result.Items.Select(Map).ToList(),
                result.TotalCount,
                result.Page,
                result.PageSize);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _getByIdHandler.Handle(new GetPetByIdQuery(id), ct);
            if (result is null)
                throw new PetNotFoundException(id);
            return Ok(Map(result));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePetRequest request, CancellationToken ct)
        {
            await _updateHandler.Handle(new UpdatePetCommand(id, request.Name, request.Species, request.BirthDate), ct);
            return Ok();
        }

        [HttpPost("{id:guid}/archive")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
        {
            await _archiveHandler.Handle(new ArchivePetCommand(id), ct);
            return Ok();
        }

        [HttpPut("{id:guid}/client")]
        public async Task<IActionResult> SetClient(Guid id, [FromBody] SetPetClientRequest request, CancellationToken ct)
        {
            await _setClientHandler.Handle(new SetPetClientCommand(id, request.ClientId), ct);
            return Ok();
        }

        private static PetResponse Map(GetPetByIdResult r) =>
            new(r.Id, r.ClientId, r.Name, r.Species, r.BirthDate, r.Status);
    }
}
