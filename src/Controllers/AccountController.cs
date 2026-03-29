using Core.Interfaces;
using Core.DTOs;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }


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