using Core.Helpers; // Priority ve FaultStatus buradan geliyor

namespace Core.Entities;

public class FaultReport
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!; // İl/İlçe/Mahalle

    // Yeni Enum yapılarınla uyumlu alanlar
    public Priority Priority { get; set; } = Priority.Medium;
    public FaultStatus Status { get; set; } = FaultStatus.YeniKayit;

    // Seeding hatasını çözen kritik alanlar
    public int UserId { get; set; } 
    public User User { get; set; } = null!; 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } // Opsiyonel güncelleme tarihi
}