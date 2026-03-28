using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FaultReport?> GetByIdAsync(int id)
    {
        return await _context.FaultReports
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<(IEnumerable<FaultReport> Items, int TotalCount)> GetPagedAsync(
        FaultStatus? status, Priority? priority, string? location, int page, int pageSize, int? createdByUserId = null)
    {
        var query = _context.FaultReports.AsQueryable();

        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (priority.HasValue) query = query.Where(x => x.Priority == priority);
        if (!string.IsNullOrEmpty(location))
            query = query.Where(x => x.Location.Contains(location));
        if (createdByUserId.HasValue)
            query = query.Where(x => x.UserId == createdByUserId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> AnyInLocationWithinHourAsync(string location)
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        return await _context.FaultReports
            .AnyAsync(x => x.Location == location && x.CreatedAt >= oneHourAgo);
    }

    public async Task AddAsync(FaultReport report)
    {
        await _context.FaultReports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FaultReport report)
    {
        _context.FaultReports.Update(report);
        await _context.SaveChangesAsync();
    }

    public async Task AddStatusLogAsync(FaultStatusLog log)
    {
        await _context.FaultStatusLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
