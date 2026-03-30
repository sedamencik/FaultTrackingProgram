namespace Core.DTOs;
using Core.Helpers;
using Core.Entities;

public class UserDto
{    
    public string UserName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public Role Role { get; set; } = Role.User;

    public ICollection<FaultReport> FaultReports { get; set; } = new List<FaultReport>();
}