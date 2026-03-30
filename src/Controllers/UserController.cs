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


    /// <summary>
    /// Updates a Fault Report.
    /// </summary>
    /// <param name="reportId">Fault Report ID.</param>
    /// <param name="report">Fault Report details.</param>
    /// <returns>Update report result.</returns>
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
    [HttpGet("reports")]
    public async Task<ActionResult<List<NotificationReadDto>>> GetReports()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
                ErrorResult error = new ErrorResult { Message = "Unauthorized access." };
                return Unauthorized(error);
        }

        var reports = await _notificationRepository.GetNotificationsForSameUser(userId);
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

        if (userIdClaim == null || report == null || report == null)
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