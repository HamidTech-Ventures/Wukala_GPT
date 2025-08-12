using LegalPlatform.Application.DTOs;
using LegalPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LegalPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup/local")]
    public async Task<IActionResult> SignUpLocal([FromBody] SignUpLocalPersonRequest request, CancellationToken ct)
    {
        var result = await _authService.SignUpLocalPersonAsync(request, ct);
        return StatusCode(result.Success ? 200 : 400, result);
    }

    [HttpPost("signup/lawyer")]
    public async Task<IActionResult> SignUpLawyer([FromBody] SignUpLawyerRequest request, CancellationToken ct)
    {
        var result = await _authService.SignUpLawyerAsync(request, ct);
        return StatusCode(result.Success ? 200 : 400, result);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request, CancellationToken ct)
    {
        var result = await _authService.VerifyOtpAsync(request, ct);
        return StatusCode(result.Success ? 200 : 400, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        return StatusCode(result.Success ? 200 : 401, result);
    }
}