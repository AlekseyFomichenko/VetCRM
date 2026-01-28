namespace VetCRM.Api.Controllers.Pets
{
    public sealed record CreatePetRequest(
        string Name,
        string Species,
        DateTime? BirthDate,
        Guid? ClientId);
}
