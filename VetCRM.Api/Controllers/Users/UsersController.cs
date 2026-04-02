using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCRM.Modules.Identity.Application.Commands;
using VetCRM.Modules.Identity.Application.Queries;
using VetCRM.Modules.Identity.Domain;
using VetCRM.SharedKernel;

namespace VetCRM.Api.Controllers.Users
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public sealed class UsersController(
        CreateUserHandler createUserHandler,
        UpdateUserHandler updateUserHandler,
        DisableUserHandler disableUserHandler,
        GetUserByIdHandler getUserByIdHandler,
        GetUsersHandler getUsersHandler) : Controller
    {
        private readonly CreateUserHandler _createUserHandler = createUserHandler;
        private readonly UpdateUserHandler _updateUserHandler = updateUserHandler;
        private readonly DisableUserHandler _disableUserHandler = disableUserHandler;
        private readonly GetUserByIdHandler _getUserByIdHandler = getUserByIdHandler;
        private readonly GetUsersHandler _getUsersHandler = getUsersHandler;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
        {
            var command = new CreateUserCommand(request.Email, request.Password, request.Role, request.FullName);
            var result = await _createUserHandler.Handle(command, ct);
            return Created($"/api/users/{result.UserId}", new { userId = result.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] string? search,
            [FromQuery] UserRole? role,
            [FromQuery] UserStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = new GetUsersQuery(search, role, status, page, pageSize);
            var result = await _getUsersHandler.Handle(query, ct);

            var response = new GetUsersResponse(
                result.Items.Select(u => new UserResponse(
                    u.Id,
                    u.Email,
                    u.Role.ToString(),
                    u.FullName,
                    u.Status.ToString(),
                    u.CreatedAt)).ToList(),
                result.TotalCount,
                result.Page,
                result.PageSize);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _getUserByIdHandler.Handle(new GetUserByIdQuery(id), ct);
            if (result is null)
                throw new UserNotFoundException(id);

            return Ok(new UserResponse(
                result.Id,
                result.Email,
                result.Role.ToString(),
                result.FullName,
                result.Status.ToString(),
                result.CreatedAt));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
        {
            var command = new UpdateUserCommand(id, request.FullName, request.Role);
            await _updateUserHandler.Handle(command, ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/disable")]
        public async Task<IActionResult> Disable(Guid id, CancellationToken ct)
        {
            var command = new DisableUserCommand(id);
            await _disableUserHandler.Handle(command, ct);
            return NoContent();
        }
    }
}
