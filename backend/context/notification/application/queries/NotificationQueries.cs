using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.notification.domain.repo;

namespace backend.context.notification.application.queries;

public record NotificationResponse(
    Guid Id,
    string UserId,
    string Type,
    string Title,
    string Message,
    string Channel,
    bool IsRead,
    DateTime? ScheduledAt,
    DateTime? SentAt,
    DateTime? FailedAt,
    string? EntityId,
    string? EntityType,
    DateTime CreatedAt
);

public record GetMyNotificationsQuery(string UserId) : IQuery<IEnumerable<NotificationResponse>>;

public class GetMyNotificationsQueryHandler : IQueryHandler<GetMyNotificationsQuery, IEnumerable<NotificationResponse>>
{
    private readonly INotificationRepository _repository;

    public GetMyNotificationsQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<NotificationResponse>> HandleAsync(GetMyNotificationsQuery query)
    {
        var notifications = await _repository.GetByUserIdAsync(query.UserId);

        return notifications.Select(n => new NotificationResponse(
            n.Id,
            n.UserId.Value.ToString(),
            n.Type.ToString(),
            n.Title,
            n.Message,
            n.Channel.ToString(),
            n.IsRead,
            n.ScheduledAt,
            n.SentAt,
            n.FailedAt,
            n.EntityId,
            n.EntityType?.ToString(),
            n.CreatedAt
        ));
    }
}
