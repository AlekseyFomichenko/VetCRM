using VetCRM.Modules.Pets.Application.Contracts;

namespace VetCRM.Modules.Pets.Application.Queries
{
    public sealed class GetPetsHandler(IPetRepository repository)
    {
        private readonly IPetRepository _repository = repository;

        public async Task<GetPetsResult> Handle(GetPetsQuery query, CancellationToken ct)
        {
            (var items, int totalCount) = await _repository.GetListAsync(
                query.Search,
                query.Page,
                query.PageSize,
                query.ClientId,
                query.Status,
                ct);

            var resultItems = items.Select(p => new GetPetByIdResult(
                p.Id,
                p.ClientId,
                p.Name,
                p.Species,
                p.BirthDate,
                p.Status)).ToList();

            return new GetPetsResult(resultItems, totalCount, query.Page, query.PageSize);
        }
    }
}
