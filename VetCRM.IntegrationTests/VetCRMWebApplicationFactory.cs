using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace VetCRM.IntegrationTests;

public sealed class VetCRMWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string JwtSecret = "VetCRM-Jwt-Secret-Key-Min32Chars-For-HmacSha256";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);
        string apiPath = GetApiProjectPath();
        if (Directory.Exists(apiPath))
            builder.UseContentRoot(apiPath);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = GetTestConnectionString(),
                ["ASPNETCORE_ENVIRONMENT"] = Environments.Development,
                ["Jwt:Secret"] = JwtSecret,
                ["Jwt:Issuer"] = "VetCRM",
                ["Jwt:Audience"] = "VetCRM",
                ["Jwt:AccessTokenExpirationMinutes"] = "15",
                ["Jwt:RefreshTokenExpirationDays"] = "7"
            });
        });
    }

    private static string GetApiProjectPath()
    {
        string fromBaseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "VetCRM.Api"));
        if (Directory.Exists(fromBaseDir))
            return fromBaseDir;
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "VetCRM.Api"));
    }

    private static string GetTestConnectionString()
    {
        string? env = Environment.GetEnvironmentVariable("VetCRM_Test_ConnectionString");
        return env ?? "Host=localhost;Port=5432;Database=vetcrm;Username=postgres;Password=postgres";
    }
}
