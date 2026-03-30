namespace Core.DTOs;
using Core.Helpers;

public class UserCreateDto
{
    
    public string UserName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string PasswordHash { get; set; } = null!; 
    
    public Role Role { get; set; } = Role.User;

}