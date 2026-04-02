using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Clients.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Clients.Application.Commands
{
    public sealed class ArchiveClientHandler(IClientRepository repository)
    {
        private readonly IClientRepository _repository = repository;

        public async Task Handle(ArchiveClientCommand command, CancellationToken ct)
        {
            Client? client = await _repository.GetByIdAsync(command.Id, ct);
            if (client is null)
                throw new ClientNotFoundException(command.Id);

            client.Archive();
            await _repository.SaveAsync(ct);
        }
    }
}
