using Core.Helpers;

namespace Core.Entities;

public class FaultStatusLog
{
    public int Id { get; set; }
    public int FaultReportId { get; set; }
    public FaultReport FaultReport { get; set; } = null!;

    public FaultStatus OldStatus { get; set; }
    public FaultStatus NewStatus { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public int ChangedById { get; set; }
    public User ChangedBy { get; set; } = null!;
}