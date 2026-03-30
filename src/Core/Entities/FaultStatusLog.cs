using System.ComponentModel.DataAnnotations.Schema;
using Core.Helpers;

namespace Core.Entities;

public class FaultStatusLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign Key (Tipinin FaultReport.Id ile aynı olduğundan emin ol - string mi int mi?)
    public string FaultReportId { get; set; } 
    
    [ForeignKey("FaultReportId")] // Bu satır EF Core'a "Uydurma, bunu kullan" der.
    public FaultReport FaultReport { get; set; }

    public FaultStatus OldStatus { get; set; }
    public FaultStatus NewStatus { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string ChangedById { get; set; }
    public User ChangedBy { get; set; } = null!;
}