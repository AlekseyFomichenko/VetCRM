using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Pets.Application.Commands
{
    public sealed class ArchivePetHandler(IPetRepository repository)
    {
        private readonly IPetRepository _repository = repository;

        public async Task Handle(ArchivePetCommand command, CancellationToken ct)
        {
            var pet = await _repository.GetByIdAsync(command.Id, ct);
            if (pet is null)
                throw new PetNotFoundException(command.Id);
            pet.Archive();
            await _repository.SaveAsync(ct);
        }
    }
}
