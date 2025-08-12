using LegalPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPlatform.Api.Controllers;

[ApiController]
[Route("api/admin/lawyers")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminController(IAdminService admin)
    {
        _admin = admin;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken ct)
    {
        var list = await _admin.GetPendingLawyersAsync(ct);
        return Ok(list);
    }

    [HttpPut("{lawyerId:guid}/verify")]
    public async Task<IActionResult> Verify([FromRoute] Guid lawyerId, CancellationToken ct)
    {
        await _admin.VerifyLawyerAsync(lawyerId, ct);
        return Ok(new { success = true });
    }

    [HttpPut("{lawyerId:guid}/reject")]
    public async Task<IActionResult> Reject([FromRoute] Guid lawyerId, [FromBody] dynamic body, CancellationToken ct)
    {
        string reason = body?.reason ?? "";
        await _admin.RejectLawyerAsync(lawyerId, reason, ct);
        return Ok(new { success = true });
    }
}