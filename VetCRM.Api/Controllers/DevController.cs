using Microsoft.AspNetCore.Mvc;
using VetCRM.Api.Services;

namespace VetCRM.Api.Controllers;

[ApiController]
[Route("api/dev")]
public sealed class DevController(IWebHostEnvironment env, DevSeedService seedService) : Controller
{
    [HttpPost("seed")]
    public async Task<IActionResult> Seed(CancellationToken ct)
    {
        if (!env.IsDevelopment())
            return NotFound();

        await seedService.RunAsync(ct);
        return Ok();
    }
}
