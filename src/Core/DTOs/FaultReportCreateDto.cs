using Core.Helpers;

namespace Core.DTOs;

/// <summary>
/// POST body: Id ve kullanıcı alanları sunucuda atanır (Id veritabanı identity).
/// </summary>
public class FaultReportCreateDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public Priority Priority { get; set; } = Priority.Medium;
}
