using System.Security.Claims;
using LegalPlatform.Application.DTOs;
using LegalPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NewsController : ControllerBase
{
    private readonly INewsService _news;
    public NewsController(INewsService news)
    {
        _news = news;
    }

    [HttpPost("interest")]
    public async Task<IActionResult> SetInterest([FromBody] NewsInterestRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue("sub")!);
        await _news.SetUserInterestAsync(userId, request.Interest, ct);
        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue("sub")!);
        var items = await _news.GetNewsAsync(userId, ct);
        return Ok(items);
    }
}