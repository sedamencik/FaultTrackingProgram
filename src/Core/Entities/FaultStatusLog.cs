using Core.Helpers;

namespace Core.Entities;

public class FaultStatusLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int FaultReportId { get; set; }
    public FaultReport FaultReport { get; set; } = null!;

    public FaultStatus OldStatus { get; set; }
    public FaultStatus NewStatus { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string ChangedById { get; set; }
    public User ChangedBy { get; set; } = null!;
}