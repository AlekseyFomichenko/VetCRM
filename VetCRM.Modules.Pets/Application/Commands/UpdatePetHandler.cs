using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Pets.Application.Commands
{
    public sealed class UpdatePetHandler(IPetRepository repository)
    {
        private readonly IPetRepository _repository = repository;

        public async Task Handle(UpdatePetCommand command, CancellationToken ct)
        {
            var pet = await _repository.GetByIdAsync(command.Id, ct);
            if (pet is null)
                throw new PetNotFoundException(command.Id);
            pet.Update(command.Name, command.Species, command.BirthDate);
            await _repository.SaveAsync(ct);
        }
    }
}
