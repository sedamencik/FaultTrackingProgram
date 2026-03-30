using Core.Interfaces;
using Core.DTOs;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]    

public class UserController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;

    public UserController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }


    /// <summary>
    /// Adds a Fault Report.
    /// </summary>
    /// <param name="report">Fault Report details.</param>
    /// <returns>Add report result.</returns>
    [HttpPost("report")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddNotification([FromQuery] NotificationCreateDto report)
    {

        // Token'dan gelen gerçek kullanıcı ID'sini alıyoruz
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    
        if (string.IsNullOrEmpty(userIdClaim))
        {
            ErrorResult error = new ErrorResult { Message = "Unauthorized access." };
            return Unauthorized(error);
        }

        if (!ModelState.IsValid)
        {
            var errorResult = new ErrorDataResult<object>
            {
                Message = "Validation errors occurred.",
                Errors = ModelState.GetErrors()
            };
            return BadRequest(errorResult);
        }

        if (userIdClaim == null || report == null)
        {
            ErrorResult error = new ErrorResult { Message = "User Id or Report empty." };
            return BadRequest(error);
        }
        
        await _notificationRepository.AddNotificationAsync(userIdClaim, report);

        SuccessResult successResponse = new SuccessResult { Message = "Notification added successfully." };
        return Ok(successResponse);
    }
    
}