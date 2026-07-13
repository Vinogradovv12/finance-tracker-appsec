using FinanceTracker.Api.Contracts.Requests;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[EnableRateLimiting("auth-policy")]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _service.RegisterUserAsync(request.Email, request.Password);

        return Ok(new { Message = "User registered successfully" }); //Using 200 to avoid leaking information about existing users
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _service.LoginUserAsync(request.Email, request.Password);

        return Ok(new { access_token = token });
    }
}