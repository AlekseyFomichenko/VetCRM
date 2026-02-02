using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Clients.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Clients.Application.Commands
{
    public sealed class CreateClientHandler(IClientRepository repository)
    {
        private readonly IClientRepository _repository = repository;

        public async Task<CreateClientResult> Handle(CreateClientCommand command, CancellationToken ct)
        {
            bool phoneExists = await _repository.ExistsByPhoneAsync(command.Phone, excludeClientId: null, ct);
            if (phoneExists)
                throw new DuplicatePhoneException(command.Phone);

            Client client = Client.Create(
                command.FullName,
                command.Phone,
                command.Email,
                command.Address,
                command.Notes);

            await _repository.AddAsync(client, ct);
            return new CreateClientResult(client.Id);
        }
    }
}
