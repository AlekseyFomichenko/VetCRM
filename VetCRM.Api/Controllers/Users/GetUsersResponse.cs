namespace VetCRM.Api.Controllers.Users
{
    public sealed record GetUsersResponse(
        IReadOnlyList<UserResponse> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
