using System.Net.Http.Json;
using System.Text.Json;
using VetCRM.Api.Controllers.Clients;
using VetCRM.Api.Controllers.Pets;
using Xunit;

namespace VetCRM.IntegrationTests;

public sealed class ClientPetFlowTests : IClassFixture<VetCRMWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ClientPetFlowTests(VetCRMWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateClient_ThenCreatePet_ThenGetClientAndPet_ReturnsExpected()
    {
        string unique = Guid.NewGuid().ToString("N")[..8];
        string email = "receptionist-" + unique + "@test.local";
        await RegisterAndLoginAsync(email, "Pass123!");

        string phone = "+7900" + unique;
        var createClientRequest = new CreateClientRequest(
            "Интеграционный Клиент",
            phone,
            email,
            null,
            null);

        using var createClientResponse = await _client.PostAsJsonAsync("/api/clients", createClientRequest);
        createClientResponse.EnsureSuccessStatusCode();
        var createClientBody = await createClientResponse.Content.ReadFromJsonAsync<CreateClientResponse>(JsonOptions);
        Assert.NotNull(createClientBody);
        Guid clientId = createClientBody.ClientId;

        var createPetRequest = new CreatePetRequest(
            "Тестовый Питомец",
            "Кот",
            DateTime.UtcNow.AddYears(-2),
            clientId);

        using var createPetResponse = await _client.PostAsJsonAsync("/api/pets", createPetRequest);
        createPetResponse.EnsureSuccessStatusCode();
        var createPetBody = await createPetResponse.Content.ReadFromJsonAsync<CreatePetResponse>(JsonOptions);
        Assert.NotNull(createPetBody);
        Guid petId = createPetBody.PetId;

        using var getClientResponse = await _client.GetAsync($"/api/clients/{clientId}");
        getClientResponse.EnsureSuccessStatusCode();
        var clientData = await getClientResponse.Content.ReadFromJsonAsync<ClientResponse>(JsonOptions);
        Assert.NotNull(clientData);
        Assert.Equal(clientId, clientData.Id);
        Assert.Equal(phone, clientData.Phone);

        using var getPetResponse = await _client.GetAsync($"/api/pets/{petId}");
        getPetResponse.EnsureSuccessStatusCode();
    }

    private async Task RegisterAndLoginAsync(string email, string password)
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
    private sealed record ClientResponse(Guid Id, string FullName, string Phone, string? Email, string? Address, string? Notes, int Status, DateTime CreatedAt);
    private sealed record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, Guid UserId, string Email, string Role);
}
