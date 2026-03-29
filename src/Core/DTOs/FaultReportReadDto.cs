using Core.Helpers;

namespace Core.DTOs;

public class FaultReportReadDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
