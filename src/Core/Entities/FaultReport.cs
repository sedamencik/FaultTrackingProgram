using System.ComponentModel.DataAnnotations.Schema;
using Core.Helpers; 

namespace Core.Entities;

public class FaultReport
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!; // İl/İlçe/Mahalle


    public Priority Priority { get; set; } = Priority.Medium;
    public FaultStatus Status { get; set; } = FaultStatus.YeniKayit;

    
    public string UserId { get; set; } 
    [ForeignKey("UserId")]
    public User User { get; set; } = null!; 


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } // Opsiyonel güncelleme tarihi
}