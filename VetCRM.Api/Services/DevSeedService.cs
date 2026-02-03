using VetCRM.Modules.Appointments.Application.Commands;
using VetCRM.Modules.Clients.Application.Commands;
using VetCRM.Modules.Clients.Application.Queries;
using VetCRM.Modules.Clients.Domain;
using VetCRM.Modules.Identity.Application.Commands;
using VetCRM.Modules.Identity.Application.Contracts;
using VetCRM.Modules.Identity.Domain;
using VetCRM.Modules.Pets.Application.Commands;

namespace VetCRM.Api.Services;

public sealed class DevSeedService(
    IUserRepository userRepository,
    CreateUserHandler createUserHandler,
    CreateClientHandler createClientHandler,
    GetClientsHandler getClientsHandler,
    CreatePetHandler createPetHandler,
    CreateAppointmentHandler createAppointmentHandler,
    CompleteAppointmentHandler completeAppointmentHandler)
{
    public const string AdminEmail = "admin@vetcrm.local";
    public const string AdminPassword = "Admin123!";
    private const string VetEmail = "vet@vetcrm.local";
    private const string VetPassword = "Vet123!";

    private readonly IUserRepository _userRepository = userRepository;
    private readonly CreateUserHandler _createUserHandler = createUserHandler;
    private readonly CreateClientHandler _createClientHandler = createClientHandler;
    private readonly GetClientsHandler _getClientsHandler = getClientsHandler;
    private readonly CreatePetHandler _createPetHandler = createPetHandler;
    private readonly CreateAppointmentHandler _createAppointmentHandler = createAppointmentHandler;
    private readonly CompleteAppointmentHandler _completeAppointmentHandler = completeAppointmentHandler;

    public async Task RunAsync(CancellationToken ct)
    {
        Guid adminId = await EnsureAdminAsync(ct);
        Guid vetId = await EnsureVeterinarianAsync(ct);
        Guid client1Id = await EnsureClientAsync("Иван Петров", "+79001110101", "ivan@example.com", ct);
        Guid client2Id = await EnsureClientAsync("Мария Сидорова", "+79001110202", null, ct);
        Guid client3Id = await EnsureClientAsync("Алексей Козлов", "+79001110303", "alex@example.com", ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        Guid pet1Id = await EnsurePetAsync("Барсик", "Кот", client1Id, today.AddYears(-3), ct);
        Guid pet2Id = await EnsurePetAsync("Шарик", "Собака", client2Id, today.AddYears(-2), ct);
        Guid pet3Id = await EnsurePetAsync("Мурка", "Кот", client1Id, today.AddYears(-1), ct);

        DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1).AddHours(10);
        DateTime tomorrow = DateTime.UtcNow.Date.AddDays(1).AddHours(10);

        Guid pastAppointmentId = await EnsureAppointmentAsync(pet1Id, client1Id, vetId, yesterday, yesterday.AddMinutes(30), "Осмотр", ct);
        await EnsureAppointmentCompletedAsync(pastAppointmentId, ct);

        await EnsureAppointmentAsync(pet2Id, client2Id, vetId, tomorrow, tomorrow.AddMinutes(30), "Прививка", ct);
    }

    private async Task<Guid> EnsureAdminAsync(CancellationToken ct)
    {
        bool exists = await _userRepository.ExistsByEmailAsync(AdminEmail, null, ct);
        if (exists)
        {
            var user = await _userRepository.GetByEmailAsync(AdminEmail, ct);
            return user!.Id;
        }
        var result = await _createUserHandler.Handle(
            new CreateUserCommand(AdminEmail, AdminPassword, UserRole.Admin, "Администратор"), ct);
        return result.UserId;
    }

    private async Task<Guid> EnsureVeterinarianAsync(CancellationToken ct)
    {
        bool exists = await _userRepository.ExistsByEmailAsync(VetEmail, null, ct);
        if (exists)
        {
            var user = await _userRepository.GetByEmailAsync(VetEmail, ct);
            return user!.Id;
        }
        var result = await _createUserHandler.Handle(
            new CreateUserCommand(VetEmail, VetPassword, UserRole.Veterinarian, "Врач Ветеринар"), ct);
        return result.UserId;
    }

    private async Task<Guid> EnsureClientAsync(string fullName, string phone, string? email, CancellationToken ct)
    {
        var list = await _getClientsHandler.Handle(
            new GetClientsQuery(phone, 1, 1, ClientStatus.Active), ct);
        if (list.TotalCount > 0)
            return list.Items[0].Id;
        var result = await _createClientHandler.Handle(
            new CreateClientCommand(fullName, phone, email, null, null), ct);
        return result.ClientId;
    }

    private async Task<Guid> EnsurePetAsync(string name, string species, Guid clientId, DateOnly? birthDate, CancellationToken ct)
    {
        var result = await _createPetHandler.Handle(
            new CreatePetCommand(clientId, name, species, birthDate), ct);
        return result.PetId;
    }

    private async Task<Guid> EnsureAppointmentAsync(
        Guid petId, Guid clientId, Guid vetId, DateTime startsAt, DateTime endsAt, string? reason, CancellationToken ct)
    {
        var result = await _createAppointmentHandler.Handle(
            new CreateAppointmentCommand(petId, clientId, vetId, startsAt, endsAt, reason), ct);
        return result.AppointmentId;
    }

    private async Task EnsureAppointmentCompletedAsync(Guid appointmentId, CancellationToken ct)
    {
        await _completeAppointmentHandler.Handle(
            new CompleteAppointmentCommand(
                appointmentId,
                "Плановый осмотр",
                "Здоров",
                "Рекомендации по питанию",
                "Витамины",
                null), ct);
    }
}
