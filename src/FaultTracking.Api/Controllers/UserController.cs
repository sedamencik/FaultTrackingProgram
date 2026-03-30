using Core.Interfaces;
using Core.DTOs;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation( Description = "Bu endpoint sadece User rolüne sahip kullanıcılar içindir. Bearer Token gereklidir."+
    " (Priority: 0-Low, 1-Medium, 2-High)")] 
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

        var isDuplicate = await _notificationRepository.AnyInLocationWithinHourAsync(report.Location);

        if (isDuplicate)
        {
            // Kural ihlali: 422 Unprocessable Entity ve açıklayıcı mesaj
            return UnprocessableEntity(new { 
               Message = "Aynı lokasyon için 1 saat içinde yalnızca bir bildirim yapılabilir. Lütfen daha sonra tekrar deneyiniz." 
            });
        }
        
        await _notificationRepository.AddNotificationAsync(userIdClaim, report);

        SuccessResult successResponse = new SuccessResult { Message = "Notification added successfully." };
        return Ok(successResponse);
    }


    /// <summary>
    /// Updates a Fault Report.
    /// </summary>
    /// <param name="reportId">Fault Report ID.</param>
    /// <param name="report">Fault Report details.</param>
    /// <returns>Update report result.</returns>
    [SwaggerOperation( Description = "Bu endpoint sadece User rolüne sahip kullanıcılar içindir. Bearer Token gereklidir."+
    " (Priority: 0-Low, 1-Medium, 2-High)")] 
    [HttpPut("report")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateNotification([FromQuery] string reportId , [FromQuery] NotificationCreateDto report)
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
        var oldReport = await _notificationRepository.GetByIdAsync(reportId);

        if (userIdClaim == null || report == null || oldReport == null)
        {
            ErrorResult error = new ErrorResult { Message = "User Id or Report empty." };
            return BadRequest(error);
        }

        if (oldReport.UserId != userIdClaim)
        {
            ErrorResult error = new ErrorResult { Message = "Unauthorized update the report." };
            return Unauthorized(error);
        }

        await _notificationRepository.UpdateNotificationAsync(oldReport.Id, report);

        SuccessResult successResponse = new SuccessResult { Message = "Notification updated successfully." };
        return Ok(successResponse);
    }

    /// <summary>
    /// Retrieves all notifications for the authenticated user.
    /// </summary>
    /// <returns>List of notifications for that user.</returns>
    /// <response code="200">Notifications retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No notification found for the authenticated user.</response>
    /// <response code="500">Failed to retrieve notification.</response>
    [SwaggerOperation( Description = "Bu endpoint sadece User rolüne sahip kullanıcılar içindir. Bearer Token gereklidir.")] 
    [HttpGet("reports")]
    public async Task<ActionResult<List<NotificationReadDto>>> GetReports([FromQuery] NotificationFilterDto filter)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
                ErrorResult error = new ErrorResult { Message = "Unauthorized access." };
                return Unauthorized(error);
        }

        var reports = await _notificationRepository.GetNotificationsForSameUser(userId, filter);
        if (reports == null || !reports.Any())
        {
                ErrorResult error = new ErrorResult { Message = "No notification found for the authenticated user." };
                return NotFound(error);
        }

        SuccessDataResult<IEnumerable<NotificationReadDto>> success = new SuccessDataResult<IEnumerable<NotificationReadDto>> { Message = "Notifications retrieved successfully.", Data = reports };
        return Ok(success);
    }

    /// <summary>
    /// Deletes a Fault Report.
    /// </summary>
    /// <param name="reportId">Fault Report ID.</param>
    /// <returns>Delete report result.</returns>
    [SwaggerOperation( Description = "Bu endpoint sadece User rolüne sahip kullanıcılar içindir. Bearer Token gereklidir.")] 
    [HttpDelete("report")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteNotification([FromQuery] string reportId)
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
        var report = await _notificationRepository.GetByIdAsync(reportId);

        if (userIdClaim == null || report == null)
        {
            ErrorResult error = new ErrorResult { Message = "User Id or Report empty." };
            return BadRequest(error);
        }

        if (report.UserId != userIdClaim)
        {
            ErrorResult error = new ErrorResult { Message = "Unauthorized delete the report." };
            return Unauthorized(error);
        }

        await _notificationRepository.DeleteAsync(report.Id);

        SuccessResult successResponse = new SuccessResult { Message = "Notification deleted successfully." };
        return Ok(successResponse);
    }

}