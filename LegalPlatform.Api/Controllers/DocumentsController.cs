using System.Security.Claims;
using LegalPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documents;

    public DocumentsController(IDocumentService documents)
    {
        _documents = documents;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0) return BadRequest(new { message = "File is required" });
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        await _documents.UploadAsync(userId, file.FileName, file.ContentType, ms.ToArray(), ct);
        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var list = await _documents.GetAllForUserAsync(userId, ct);
        return Ok(list);
    }

    [HttpGet("{documentId:guid}/download")]
    public async Task<IActionResult> Download([FromRoute] Guid documentId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var (name, contentType, data) = await _documents.DownloadAsync(userId, documentId, ct);
        return File(data, contentType, name);
    }
}