using Core.DTOs;
using Core.Entities;
using Core.Helpers;

namespace Core.Interfaces;

public interface INotificationRepository
{
    Task AddNotificationAsync(string userId, NotificationCreateDto report);//User can use
    Task<NotificationReadDto> UpdateNotificationAsync(string reportId, NotificationCreateDto report);//User can use
    Task<IEnumerable<NotificationReadDto>> GetNotificationsForSameUser(string userId);//User can use
    Task<IEnumerable<NotificationReadDto>> GetAllNotificationsAsync();//Admin can use


    Task<FaultReport?> GetByIdAsync(string id);

    //Task<(IEnumerable<FaultReport> Items, int TotalCount)> GetPagedAsync(FaultStatus? status, Priority? priority, string? location, int page, int pageSize, int? createdByUserId = null);

    Task<bool> AnyInLocationWithinHourAsync(string location);
    bool IsStatusTransitionValid(FaultStatus currentStatus, FaultStatus nextStatus);
    Task UpdateStatusAsync(string id, FaultStatus currentStatus);
    Task DeleteAsync(string id);
    //Task AddStatusLogAsync(FaultStatusLog log);

}