using VetCRM.Modules.Pets.Application.Contracts;

namespace VetCRM.Modules.Pets.Application.Queries
{
    public sealed class GetPetByIdHandler(IPetRepository repository)
    {
        private readonly IPetRepository _repository = repository;

        public async Task<GetPetByIdResult?> Handle(GetPetByIdQuery query, CancellationToken ct)
        {
            var pet = await _repository.GetByIdAsync(query.Id, ct);
            if (pet is null)
                return null;
            return new GetPetByIdResult(
                pet.Id,
                pet.ClientId,
                pet.Name,
                pet.Species,
                pet.BirthDate,
                pet.Status);
        }
    }
}
