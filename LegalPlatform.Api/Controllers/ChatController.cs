using System.Security.Claims;
using LegalPlatform.Application.DTOs;
using LegalPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chat;
    public ChatController(IChatService chat)
    {
        _chat = chat;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var answer = await _chat.AskAiAsync(userId, request.Query, ct);
        return Ok(new { response = answer });
    }

    [HttpPost("secure")]
    [Authorize(Roles = "LocalPerson,Lawyer")]
    public async Task<IActionResult> SendSecure([FromBody] ChatRequest request, [FromQuery] Guid? conversationId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var convId = await _chat.SendSecureMessageAsync(conversationId ?? Guid.Empty, userId, request.Query, ct);
        return Ok(new { conversationId = convId });
    }

    [HttpPost("secure/file")]
    [Authorize(Roles = "LocalPerson,Lawyer")]
    public async Task<IActionResult> SendSecureFile([FromForm] IFormFile file, [FromQuery] Guid? conversationId, CancellationToken ct)
    {
        if (file == null || file.Length == 0) return BadRequest(new { message = "File is required" });
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        var convId = await _chat.SendSecureFileAsync(conversationId ?? Guid.Empty, userId, file.FileName, file.ContentType, ms.ToArray(), ct);
        return Ok(new { conversationId = convId });
    }

    [HttpGet("secure/{conversationId:guid}")]
    [Authorize(Roles = "LocalPerson,Lawyer")]
    public async Task<IActionResult> GetSecure([FromRoute] Guid conversationId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var items = await _chat.GetSecureConversationAsync(conversationId, userId, ct);
        return Ok(items.Select(i => new { createdAt = i.createdAt, message = i.message }));
    }
}