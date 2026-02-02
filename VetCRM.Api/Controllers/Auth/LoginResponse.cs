namespace VetCRM.Api.Controllers.Auth
{
    public sealed record LoginResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        Guid UserId,
        string Email,
        string Role);
}
