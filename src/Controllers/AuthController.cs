using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.UserName, request.Password);
        
        if (!result.Success)
            return Unauthorized(result); // 401 Hatası

        return Ok(result); // 200 OK ve Token
    }
}

// Bssic DTO (Data Transfer Object)
public record LoginRequest(string UserName, string Password);