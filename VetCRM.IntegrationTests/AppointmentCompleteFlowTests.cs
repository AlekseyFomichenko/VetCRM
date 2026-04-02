using System.Net.Http.Json;
using System.Text.Json;
using VetCRM.Api.Controllers.Appointments;
using VetCRM.Api.Controllers.Clients;
using VetCRM.Api.Controllers.Pets;
using Xunit;

namespace VetCRM.IntegrationTests;

public sealed class AppointmentCompleteFlowTests : IClassFixture<VetCRMWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AppointmentCompleteFlowTests(VetCRMWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAppointment_Complete_ThenGetMedicalRecordsByPetId_ReturnsRecord()
    {
        string unique = Guid.NewGuid().ToString("N")[..8];
        (Guid vetId, string vetToken) = await RegisterVetAndGetTokenAsync("vet-" + unique + "@test.local", "Vet123!");
        await RegisterReceptionistAndLoginAsync("receptionist-" + unique + "@test.local", "Pass123!");
        string phone = "+7900" + unique;
        var createClientRequest = new CreateClientRequest("Владелец Приёма", phone, "apt-" + unique + "@test.local", null, null);
        using var cr = await _client.PostAsJsonAsync("/api/clients", createClientRequest);
        cr.EnsureSuccessStatusCode();
        var clientBody = await cr.Content.ReadFromJsonAsync<CreateClientResponse>(JsonOptions);
        Assert.NotNull(clientBody);
        Guid clientId = clientBody.ClientId;

        var createPetRequest = new CreatePetRequest("Питомец Приёма", "Собака", DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1), clientId);
        using var pr = await _client.PostAsJsonAsync("/api/pets", createPetRequest);
        pr.EnsureSuccessStatusCode();
        var petBody = await pr.Content.ReadFromJsonAsync<CreatePetResponse>(JsonOptions);
        Assert.NotNull(petBody);
        Guid petId = petBody.PetId;

        DateTime startsAt = DateTime.UtcNow.AddHours(-2);
        DateTime endsAt = startsAt.AddMinutes(30);
        var createAppointmentRequest = new CreateAppointmentRequest(petId, clientId, vetId, startsAt, endsAt, "Осмотр");
        using var ar = await _client.PostAsJsonAsync("/api/appointments", createAppointmentRequest);
        ar.EnsureSuccessStatusCode();
        var appointmentBody = await ar.Content.ReadFromJsonAsync<CreateAppointmentResponse>(JsonOptions);
        Assert.NotNull(appointmentBody);
        Guid appointmentId = appointmentBody.AppointmentId;

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", vetToken);
        var completeRequest = new CompleteAppointmentRequest(
            "Жалоба",
            "Диагноз",
            "План лечения",
            "Назначения",
            null);
        using var completeResponse = await _client.PutAsJsonAsync($"/api/appointments/{appointmentId}/complete", completeRequest);
        completeResponse.EnsureSuccessStatusCode();

        using var medicalResponse = await _client.GetAsync($"/api/pets/{petId}/medical-records");
        medicalResponse.EnsureSuccessStatusCode();
        var records = await medicalResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.True(records.GetArrayLength() >= 1);
    }

    private async Task<(Guid VetId, string VetToken)> RegisterVetAndGetTokenAsync(string email, string password)
    {
        var registerRequest = new { email, password, role = 1 };
        using var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        var loginRequest = new { email, password };
        using var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        Assert.NotNull(loginBody?.AccessToken);
        return (loginBody.UserId, loginBody.AccessToken);
    }

    private async Task RegisterReceptionistAndLoginAsync(string email, string password)
    {
        var registerRequest = new { email, password, role = 2 };
        using var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        var loginRequest = new { email, password };
        using var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        Assert.NotNull(loginBody?.AccessToken);
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginBody.AccessToken);
    }

    private sealed record CreateClientResponse(Guid ClientId);
    private sealed record CreatePetResponse(Guid PetId);
    private sealed record CreateAppointmentResponse(Guid AppointmentId);
    private sealed record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, Guid UserId, string Email, string Role);
}
