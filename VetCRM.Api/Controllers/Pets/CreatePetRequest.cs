namespace VetCRM.Api.Controllers.Pets
{
    public sealed record CreatePetRequest(
        string Name,
        string Species,
        DateOnly? BirthDate,
        Guid? ClientId);
}
