using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.context.notification.domain.entity;

namespace backend.context.notification.domain.repo;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<Notification?> GetByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, int limit = 50);
    Task UpdateAsync(Notification notification);
}
