using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.Modules.Pets.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Pets.Application.Commands
{
    public sealed class CreatePetHandler(IPetRepository pets, IClientReadService clients)
    {
        private readonly IPetRepository _pets = pets;
        private readonly IClientReadService _clients = clients;

        public async Task<CreatePetResult> Handle(CreatePetCommand command, CancellationToken ct)
        {
            if (command.ClientId.HasValue)
            {
                bool exist = await _clients.ExistsAsync(command.ClientId.Value, ct);
                if (!exist) throw new ClientNotFoundException(command.ClientId.Value);
            }
            Pet pet = Pet.Create(
                command.ClientId,
                command.Name,
                command.Species,
                command.BirthDate);

            await _pets.AddAsync(pet, ct);
            return new CreatePetResult(pet.Id);
        }

    }
}
