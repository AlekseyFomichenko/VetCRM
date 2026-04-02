using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Clients.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Clients.Application.Commands
{
    public sealed class UpdateClientHandler(IClientRepository repository)
    {
        private readonly IClientRepository _repository = repository;

        public async Task Handle(UpdateClientCommand command, CancellationToken ct)
        {
            Client? client = await _repository.GetByIdAsync(command.Id, ct);
            if (client is null)
                throw new ClientNotFoundException(command.Id);

            bool phoneExists = await _repository.ExistsByPhoneAsync(command.Phone, command.Id, ct);
            if (phoneExists)
                throw new DuplicatePhoneException(command.Phone);

            client.Update(
                command.FullName,
                command.Phone,
                command.Email,
                command.Address,
                command.Notes);

            await _repository.SaveAsync(ct);
        }
    }
}
