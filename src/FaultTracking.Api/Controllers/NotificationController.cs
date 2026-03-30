using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;
/*
[Authorize] // Token'sız kimse giremez
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Token içinden rolü ve kullanıcı ID'sini alıyoruz
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Gereksinim #1: User sadece kendisininkini, Admin hepsini görür
        // Bu mantığı Servis katmanında handle edeceğiz
        var result = await _service.GetNotificationsAsync(userRole, userId, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FaultReport report)
    {
        report.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.CreateNotificationAsync(report);
        
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetAll), new { id = result.Data?.Id }, result);
    }

    [Authorize(Roles = "Admin")] // Sadece Admin girebilir (Gereksinim #3)
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] FaultStatus newStatus)
    {
        var changedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.UpdateStatusAsync(id, newStatus, changedByUserId);
        
        if (!result.Success) return UnprocessableEntity(result); // 422 Hatası (Gereksinim #5)
        return Ok(result);
    }
}*/