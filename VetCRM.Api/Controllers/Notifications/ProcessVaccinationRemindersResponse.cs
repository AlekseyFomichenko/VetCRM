namespace VetCRM.Api.Controllers.Notifications
{
    public sealed record ProcessVaccinationRemindersResponse(int Created, int Sent, int Failed);
}
