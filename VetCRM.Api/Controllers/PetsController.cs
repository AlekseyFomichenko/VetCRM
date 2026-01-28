using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using VetCRM.Api.Controllers.Pets;
using VetCRM.Modules.Pets.Application.Commands;

namespace VetCRM.Api.Controllers
{
    [ApiController]
    [Route("api/pets")]
    public sealed class PetsController(CreatePetHandler handler) : Controller
    {
        private readonly CreatePetHandler _handler = handler;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePetRequest request, CancellationToken ct)
        {
            var command = new CreatePetCommand(Name: request.Name,
                                               Species: request.Species,
                                               BirthDate: request.BirthDate,
                                               ClientId: request.ClientId);

            var result = await _handler.Handle(command, ct);

            return CreatedAtAction(nameof(GetById),
                                   new { id = result.PetId },
                                   result);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id) 
        {
            return Ok();
        }
    }
}
