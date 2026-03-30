using Core.Helpers;
namespace Core.DTOs;
public class NotificationFilterDto
{
    public FaultStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public string? Location { get; set; }
    
    // Sayfalama
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Sıralama (Örn: "priority" veya "createdAt")
    public string? SortBy { get; set; } 
}