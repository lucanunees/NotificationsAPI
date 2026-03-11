using Microsoft.EntityFrameworkCore;
using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Interfaces;
using NotificationsAPI.Infrastructure.Persistence;

namespace NotificationsAPI.Infrastructure.Repositories;

/// <summary>
/// Repositório de notificações com Entity Framework Core
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
