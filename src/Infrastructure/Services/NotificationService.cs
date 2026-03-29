using Core.DTOs;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;

namespace Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;

    public NotificationService(INotificationRepository repo)
    {
        _repo = repo;
    }

    public Task<ApiResponse<FaultReport>> CreateNotificationAsync(FaultReport report)
    {
        throw new NotImplementedException();
    }

    public Task<NotificationPagedResult> GetNotificationsAsync(string? userRole, int userId, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<FaultReport>> UpdateStatusAsync(int id, FaultStatus newStatus, int changedByUserId)
    {
        throw new NotImplementedException();
    }

    /*
    public async Task<NotificationPagedResult> GetNotificationsAsync(string? userRole, int userId, int page, int pageSize)
    {
        int? createdBy = string.Equals(userRole, nameof(Role.Admin), StringComparison.OrdinalIgnoreCase)
            ? null
            : userId;

        var (items, total) = await _repo.GetPagedAsync(null, null, null, page, pageSize, createdBy);
        return new NotificationPagedResult(items, total, page, pageSize);
    }

    public async Task<ApiResponse<FaultReport>> CreateNotificationAsync(FaultReport report)
    {
        if (await _repo.AnyInLocationWithinHourAsync(report.Location))
            return ApiResponse<FaultReport>.Fail("Bu konumda son 1 saat içinde zaten bir kayıt var.");

        await _repo.AddAsync(report);
        return ApiResponse<FaultReport>.Ok(report);
    }

    public async Task<ApiResponse<FaultReport>> UpdateStatusAsync(int id, FaultStatus newStatus, int changedByUserId)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
            return ApiResponse<FaultReport>.Fail("Kayıt bulunamadı.");

        var old = existing.Status;
        if (old == newStatus)
            return ApiResponse<FaultReport>.Ok(existing);

        existing.Status = newStatus;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repo.AddStatusLogAsync(new FaultStatusLog
        {
            FaultReportId = existing.Id,
            OldStatus = old,
            NewStatus = newStatus,
            ChangedById = changedByUserId
        });

        await _repo.UpdateAsync(existing);
        return ApiResponse<FaultReport>.Ok(existing);
    }*/
}
