using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Entities;
using Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;


public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<string> LoginAsync(LoginDto login)
    {
        // 1. Kullanıcıyı bul (Şu an şifre hash kontrolü yapmıyoruz, direkt eşleşmeye bakıyoruz)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == login.Email && u.PasswordHash == login.Password);

        if (user == null) return "";

        // 2. Token oluştur
        var token = GenerateJwtToken(user);

        return token;
    }

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Token içine kullanıcının kimlik bilgilerini (Claims) gömüyoruz
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
