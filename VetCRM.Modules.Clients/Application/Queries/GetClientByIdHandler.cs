using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Clients.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Clients.Application.Queries
{
    public sealed class GetClientByIdHandler(IClientRepository repository)
    {
        private readonly IClientRepository _repository = repository;

        public async Task<GetClientByIdResult?> Handle(GetClientByIdQuery query, CancellationToken ct)
        {
            Client? client = await _repository.GetByIdAsync(query.Id, ct);
            if (client is null)
                return null;

            return new GetClientByIdResult(
                client.Id,
                client.FullName,
                client.Phone,
                client.Email,
                client.Address,
                client.Notes,
                client.Status,
                client.CreatedAt);
        }
    }
}
