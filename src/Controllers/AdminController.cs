using Core.Interfaces;
using Core.DTOs;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]    
public class AdminController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public AdminController(INotificationRepository notificationRepository, IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
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
    /// Retrieves all notifications.
    /// </summary>
    /// <returns>List of notifications for all users.</returns>
    /// <response code="200">Notifications retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No notification found.</response>
    /// <response code="500">Failed to retrieve notification.</response>
    [HttpGet("reports")]
    public async Task<ActionResult<List<NotificationReadDto>>?> GetReports()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
                ErrorResult error = new ErrorResult { Message = "Unauthorized access." };
                return Unauthorized(error);
        }

        var reports = await _notificationRepository.GetAllNotificationsAsync();
        if (reports == null || !reports.Any())
        {
                ErrorResult error = new ErrorResult { Message = "No notification found." };
                return NotFound(error);
        }

        SuccessDataResult<IEnumerable<NotificationReadDto>> success = new SuccessDataResult<IEnumerable<NotificationReadDto>> { Message = "Notifications retrieved successfully.", Data = reports };
        return Ok(success);
    }




    /// <summary>
    /// Retrieves user's notifications.
    /// </summary>
    /// <returns>List of notifications for that users.</returns>
    /// <response code="200">Notifications retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No notification found.</response>
    /// <response code="500">Failed to retrieve notification.</response>
    [HttpGet("userReports")]
    public async Task<ActionResult<List<NotificationReadDto>>?> GetNotificationsForSameUser([FromQuery] string userId)
    {
        var reports = await _notificationRepository.GetNotificationsForSameUser(userId);
        if (reports == null || !reports.Any())
        {
                ErrorResult error = new ErrorResult { Message = "No notification found for that user." };
                return NotFound(error);
        }

        SuccessDataResult<IEnumerable<NotificationReadDto>> success = new SuccessDataResult<IEnumerable<NotificationReadDto>> { Message = "Notifications retrieved successfully.", Data = reports };
        return Ok(success);
    }

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>List of users.</returns>
    /// <response code="200">Users retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No user found.</response>
    /// <response code="500">Failed to retrieve user.</response>
    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>?> GetUsers()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
                ErrorResult error = new ErrorResult { Message = "Unauthorized access." };
                return Unauthorized(error);
        }

        var users = await _userRepository.GetAllAsync();
        if (users == null || !users.Any())
        {
                ErrorResult error = new ErrorResult { Message = "No user found." };
                return NotFound(error);
        }

        SuccessDataResult<IEnumerable<UserDto>> success = new SuccessDataResult<IEnumerable<UserDto>> { Message = "Users retrieved successfully.", Data = users };
        return Ok(success);
    }

    /// <summary>
    /// Adds a User.
    /// </summary>
    /// <param name="report">User details.</param>
    /// <returns>Add user result.</returns>
    [HttpPost("user")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddUser([FromQuery] UserCreateDto user)
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

        if (userIdClaim == null || user == null)
        {
            ErrorResult error = new ErrorResult { Message = "User infos empty." };
            return BadRequest(error);
        }
        
        await _userRepository.CreateAsync(user);

        SuccessResult successResponse = new SuccessResult { Message = "User created successfully." };
        return Ok(successResponse);
    }

     /// <summary>
    /// Updates a User.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="user">User details.</param>
    /// <returns>Update user result.</returns>
    [HttpPut("user")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateNotification([FromQuery] string userId , [FromQuery] UserCreateDto user)
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
        var oldUser = await _userRepository.GetByIdAsync(userId);

        if (userIdClaim == null || oldUser == null)
        {
            ErrorResult error = new ErrorResult { Message = "User Id or User model empty." };
            return BadRequest(error);
        }

        await _userRepository.UpdateAsync(oldUser.Id, user);

        SuccessResult successResponse = new SuccessResult { Message = "User updated successfully." };
        return Ok(successResponse);
    }

    /// <summary>
    /// Deletes a User.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns>Delete report result.</returns>
    [HttpDelete("user")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteUser([FromQuery] string userId)
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
        var user = await _userRepository.GetByIdAsync(userId);

        if (userIdClaim == null || user == null || userId == null)
        {
            ErrorResult error = new ErrorResult { Message = "User Id or User Model empty." };
            return BadRequest(error);
        }

        await _userRepository.DeleteAsync(userId);

        SuccessResult successResponse = new SuccessResult { Message = "User deleted successfully." };
        return Ok(successResponse);
    }
}