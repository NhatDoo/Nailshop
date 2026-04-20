using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.notification.domain.entity;
using backend.context.notification.domain.repo;
using Microsoft.EntityFrameworkCore;

namespace backend.context.notification.infrastructure.persistence;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, int limit = 50)
    {
        return await _context.Notifications
            .Where(n => n.UserId.Value.ToString() == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }
}
