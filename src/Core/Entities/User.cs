namespace Core.Entities;
using Core.Helpers;

public class User
{
    public int Id { get; set; }
    
    public string UserName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    // Güvenlik için PasswordHash saklamak doğru yaklaşımdır
    public string PasswordHash { get; set; } = null!; 
    
    // Gereksinim: Admin ve User rolleri
    public Role Role { get; set; } = Role.User;

    // İlişki: Bir kullanıcının birden fazla bildirimi olabilir
    public ICollection<FaultReport> FaultReports { get; set; } = new List<FaultReport>();
}