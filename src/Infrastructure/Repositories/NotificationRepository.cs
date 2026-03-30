using Core.DTOs;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;


namespace Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;


    public NotificationRepository(AppDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }



    public async Task AddNotificationAsync(string userId, NotificationCreateDto report)
    {
        var newReport = new FaultReport {
            Title = report.Title,
            Description = report.Description,
            Location = report.Location,
            Priority = report.Priority,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
        };
        await _context.FaultReports.AddAsync(newReport);
        await _context.SaveChangesAsync();
    }

    public async Task<NotificationReadDto> UpdateNotificationAsync(string reportId, NotificationCreateDto report)
    {
        var existingReport = await _context.FaultReports.FindAsync(reportId);
        if (existingReport == null)
        {
            throw new ArgumentException($"No report found with ID '{reportId}'.");
        }
        existingReport.Title = report.Title;
        existingReport.Description = report.Description;
        existingReport.Location = report.Location;
        existingReport.Priority = report.Priority;
        existingReport.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<NotificationReadDto>(existingReport);
    }

    public async Task<IEnumerable<NotificationReadDto>> GetNotificationsForSameUser(string userId)
    {
        var userFound = await _context.Users.AnyAsync(r => r.Id == userId);
        if (!userFound)
        {
            throw new ArgumentException("User cannot found.");
        }
        var reports = await _context.FaultReports
            .Where(s => s.UserId == userId)
            .Include(x => x.User) // Bu satır kritik!
            .ToListAsync();

        return _mapper.Map<IEnumerable<NotificationReadDto>>(reports);
    }

    public async Task<IEnumerable<NotificationReadDto>> GetAllNotificationsAsync()
    {
        var reports = await _context.FaultReports
            .Include(r => r.User) // FaultReport içindeki 'User' property'sini doldurur
            .AsNoTracking()
            .ToListAsync();
        return _mapper.Map<IEnumerable<NotificationReadDto>>(reports);    
    }
    
    public async Task<FaultReport?> GetByIdAsync(string id)
    {
        return await _context.FaultReports
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    /*public async Task<(IEnumerable<FaultReport> Items, int TotalCount)> GetPagedAsync(
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
    }*/

    public async Task<bool> AnyInLocationWithinHourAsync(string location)
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        return await _context.FaultReports
            .AnyAsync(x => x.Location == location && x.CreatedAt >= oneHourAgo);
    }

    public async Task DeleteAsync(string id)
    {
        var report = await _context.FaultReports.FirstOrDefaultAsync(s => s.Id == id );
            
        if (report == null)
        {
            throw new ArgumentException($"No report found with ID '{id}'.");
        }

        _context.FaultReports.Remove(report);
        await _context.SaveChangesAsync();    
    }

    /*public async Task AddStatusLogAsync(FaultStatusLog log)
    {
        await _context.FaultStatusLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }*/


    public bool IsStatusTransitionValid(FaultStatus currentStatus, FaultStatus nextStatus)
    {
        return (currentStatus, nextStatus) switch
        {
            (FaultStatus.YeniKayit, FaultStatus.Inceleniyor) => true,            
            (FaultStatus.YeniKayit, FaultStatus.Iptal) => true,
            (FaultStatus.Inceleniyor, FaultStatus.Atandi) => true,
            (FaultStatus.Inceleniyor, FaultStatus.Asilsiz) => true,
            (FaultStatus.Inceleniyor, FaultStatus.Iptal) => true,
            (FaultStatus.Atandi, FaultStatus.Calisiliyor) => true,
            (FaultStatus.Atandi, FaultStatus.Iptal) => true,
            (FaultStatus.Calisiliyor, FaultStatus.Tamamlandi) => true,
            (FaultStatus.Calisiliyor, FaultStatus.Iptal) => true,

            // Geçersiz geçişler:
            (FaultStatus.Tamamlandi, _) => false, // Çözülmüş bir kayıt değiştirilemez
            (FaultStatus.Iptal, _) => false, // İptal edilmiş bir kayıt değiştirilemez
            (FaultStatus.Asilsiz, _) => false, // İptal edilmiş bir kayıt değiştirilemez
            _ => false
        };
    }

    public async Task UpdateStatusAsync(string id, FaultStatus currentStatus)
    {
        var existingReport = await _context.FaultReports.FindAsync(id);
        if (existingReport == null)
        {
            throw new ArgumentException($"No report found with ID '{id}'.");
        }
        existingReport.Status = currentStatus;
        existingReport.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
