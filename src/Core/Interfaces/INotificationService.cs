using Core.DTOs;
using Core.Entities;
using Core.Helpers;

namespace Core.Interfaces;

public interface INotificationRepository
{
    Task<FaultReportReadDto?> GetByIdAsync(int id);

    Task<(IEnumerable<FaultReport> Items, int TotalCount)> GetPagedAsync(
        FaultStatus? status, Priority? priority, string? location, int page, int pageSize, int? createdByUserId = null);

    Task AddAsync(FaultReport report);
    Task UpdateAsync(FaultReport report);

    Task<bool> AnyInLocationWithinHourAsync(string location);

    Task AddStatusLogAsync(FaultStatusLog log);
}

public interface INotificationService
{
    Task<NotificationPagedResult> GetNotificationsAsync(string? userRole, int userId, int page, int pageSize);
    Task<ApiResponse<FaultReport>> CreateNotificationAsync(FaultReport report);
    Task<ApiResponse<FaultReport>> UpdateStatusAsync(int id, FaultStatus newStatus, int changedByUserId);
}