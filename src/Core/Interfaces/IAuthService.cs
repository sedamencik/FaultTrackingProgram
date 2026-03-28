using Core.Entities;
using Core.DTOs;

namespace Core.Interfaces;

public interface IAuthService
{
    // Kullanıcı adı ve şifre ile giriş
    Task<ApiResponse<string>> LoginAsync(string username, string password);
    
    // User nesnesinden JWT Token üretme
    string GenerateJwtToken(User user);
}