namespace VetCRM.Api.Controllers.Pets
{
    public sealed record UpdatePetRequest(
        string Name,
        string Species,
        DateOnly? BirthDate);
}
