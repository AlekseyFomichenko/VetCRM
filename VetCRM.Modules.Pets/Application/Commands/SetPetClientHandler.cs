using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Pets.Application.Commands
{
    public sealed class SetPetClientHandler(IPetRepository petRepository, IClientReadService clientReadService)
    {
        private readonly IPetRepository _petRepository = petRepository;
        private readonly IClientReadService _clientReadService = clientReadService;

        public async Task Handle(SetPetClientCommand command, CancellationToken ct)
        {
            var pet = await _petRepository.GetByIdAsync(command.PetId, ct);
            if (pet is null)
                throw new PetNotFoundException(command.PetId);

            if (command.ClientId.HasValue)
            {
                bool exists = await _clientReadService.ExistsAsync(command.ClientId.Value, ct);
                if (!exists)
                    throw new ClientNotFoundException(command.ClientId.Value);
                pet.ChangeClient(command.ClientId.Value);
            }
            else
            {
                pet.UnassignFromClient();
            }

            await _petRepository.SaveAsync(ct);
        }
    }
}
