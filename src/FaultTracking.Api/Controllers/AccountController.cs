using Core.Interfaces;
using Core.DTOs;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

[EnableRateLimiting("fixed")]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }


    /// <summary>
    ///  Click to login seeded email and password of user types, and log in as one.
    /// </summary>
    [SwaggerOperation(
        Summary = "Seeded kullanıcılarla giriş yapın",
        Description = "User: zeynep@sirket.com / pass123 | Admin: caner@sirket.com / hash123"
    )]    
    /// <returns>A response containing the authentication token.</returns>
    [HttpPost("login")]    
    public async Task<ActionResult<string>> Login([FromQuery] LoginDto request)
    {
        var token = await _authService.LoginAsync(request);
        
        if (token == "")
        {
            return Unauthorized(new ErrorResult { Message = "E-posta veya şifre hatalı." });
        }

        SuccessDataResult<string> success = new SuccessDataResult<string> { Message = "Token başarıyla oluşturuldu.", Data = token };
        return Ok(success);    
    }
    
}