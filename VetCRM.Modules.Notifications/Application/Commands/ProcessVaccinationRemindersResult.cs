namespace VetCRM.Modules.Notifications.Application.Commands
{
    public sealed record ProcessVaccinationRemindersResult(int Created, int Sent, int Failed);
}
