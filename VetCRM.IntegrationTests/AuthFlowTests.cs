using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace VetCRM.IntegrationTests;

public sealed class AuthFlowTests : IClassFixture<VetCRMWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AuthFlowTests(VetCRMWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ThenLogin_ThenCallProtectedApi_ReturnsSuccess()
    {
        string unique = Guid.NewGuid().ToString("N")[..8];
        string email = "receptionist-" + unique + "@test.local";
        string password = "Pass123!";

        var registerRequest = new { email, password, role = 2 };
        using var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new { email, password };
        using var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        Assert.NotNull(loginBody?.AccessToken);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginBody.AccessToken);

        using var getClientsResponse = await _client.GetAsync("/api/clients?page=1&pageSize=5");
        getClientsResponse.EnsureSuccessStatusCode();
    }

    private sealed record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, Guid UserId, string Email, string Role);
}
