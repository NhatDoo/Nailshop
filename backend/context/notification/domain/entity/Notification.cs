using System;
using backend.context.common.domain;
using backend.context.notification.domain.vo;
using backend.context.identity.domain.vo;

namespace backend.context.notification.domain.entity;

public class Notification : AggregateRoot
{
    public Guid Id { get; private set; }
    public UserIdVO UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public string? EntityId { get; private set; }
    public NotificationEntityType? EntityType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Notification()
    {
        UserId = null!;
        Title = null!;
        Message = null!;
    } // EF Core

    public static Notification Create(
        UserIdVO userId,
        NotificationType type,
        string title,
        string message,
        NotificationChannel channel,
        DateTime? scheduledAt = null,
        string? entityId = null,
        NotificationEntityType? entityType = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Channel = channel,
            IsRead = false,
            ScheduledAt = scheduledAt,
            EntityId = entityId,
            EntityType = entityType,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }

    public void MarkAsSent(DateTime sentAt)
    {
        SentAt = sentAt;
    }

    public void MarkAsFailed(DateTime failedAt)
    {
        FailedAt = failedAt;
    }
}
