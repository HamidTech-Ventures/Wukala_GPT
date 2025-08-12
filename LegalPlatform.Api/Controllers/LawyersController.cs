using System.Security.Claims;
using LegalPlatform.Application.DTOs;
using LegalPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LawyersController : ControllerBase
{
    private readonly ILawyerService _lawyers;

    public LawyersController(ILawyerService lawyers)
    {
        _lawyers = lawyers;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetApproved(CancellationToken ct)
    {
        var list = await _lawyers.GetApprovedLawyersAsync(ct);
        return Ok(list);
    }

    [HttpPut("update")]
    [Authorize(Roles = "Lawyer")]
    public async Task<IActionResult> Update([FromBody] SignUpLawyerRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        await _lawyers.UpdateLawyerProfileAsync(userId, request, ct);
        return Ok(new { success = true });
    }
}