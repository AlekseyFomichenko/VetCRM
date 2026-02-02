using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Clients.Domain;

namespace VetCRM.Modules.Clients.Application.Queries
{
    public sealed class GetClientsHandler(IClientRepository repository)
    {
        private readonly IClientRepository _repository = repository;

        public async Task<GetClientsResult> Handle(GetClientsQuery query, CancellationToken ct)
        {
            (IReadOnlyList<Client> items, int totalCount) = await _repository.GetListAsync(
                query.Search,
                query.Page,
                query.PageSize,
                query.Status,
                ct);

            var resultItems = items.Select(c => new GetClientByIdResult(
                c.Id,
                c.FullName,
                c.Phone,
                c.Email,
                c.Address,
                c.Notes,
                c.Status,
                c.CreatedAt)).ToList();

            return new GetClientsResult(
                resultItems,
                totalCount,
                query.Page,
                query.PageSize);
        }
    }
}
