using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.identity.domain.vo;
using backend.context.notification.domain.entity;
using backend.context.notification.domain.repo;
using backend.context.notification.domain.vo;
using backend.context.notification.application.services;
using backend.context.identity.domain.repo;

namespace backend.context.notification.application.commands;

public record CreateNotificationCommand(
    string UserId,
    string Type,
    string Title,
    string Message,
    string Channel,
    DateTime? ScheduledAt = null,
    string? EntityId = null,
    string? EntityType = null
) : ICommand<Guid>;

public record MarkNotificationAsReadCommand(
    Guid Id,
    string UserId
) : ICommand<bool>;

public class CreateNotificationCommandHandler : ICommandHandler<CreateNotificationCommand, Guid>
{
    private readonly INotificationRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public CreateNotificationCommandHandler(
        INotificationRepository repository, 
        IEmailService emailService,
        IUserRepository userRepository)
    {
        _repository = repository;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<Guid> HandleAsync(CreateNotificationCommand command)
    {
        if (!Enum.TryParse<NotificationType>(command.Type, true, out var type))
            throw new ArgumentException("Notification Type không hợp lệ.");

        if (!Enum.TryParse<NotificationChannel>(command.Channel, true, out var channel))
            throw new ArgumentException("Notification Channel không hợp lệ.");

        NotificationEntityType? entityType = null;
        if (!string.IsNullOrEmpty(command.EntityType))
        {
            if (!Enum.TryParse<NotificationEntityType>(command.EntityType, true, out var eType))
                throw new ArgumentException("Entity Type không hợp lệ.");
            entityType = eType;
        }

        var notification = Notification.Create(
            new UserIdVO(Guid.Parse(command.UserId)),
            type,
            command.Title,
            command.Message,
            channel,
            command.ScheduledAt,
            command.EntityId,
            entityType
        );

        await _repository.AddAsync(notification);

        // Nơi gửi Mail thực tế (trực tiếp nếu không scheduled)
        if (channel == NotificationChannel.Email && (!command.ScheduledAt.HasValue || command.ScheduledAt.Value <= DateTime.UtcNow))
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(new UserIdVO(Guid.Parse(command.UserId)));
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    await _emailService.SendEmailAsync(user.Email, command.Title, command.Message);
                    notification.MarkAsSent(DateTime.UtcNow);
                    await _repository.UpdateAsync(notification);
                }
            }
            catch
            {
                notification.MarkAsFailed(DateTime.UtcNow);
                await _repository.UpdateAsync(notification);
            }
        }

        return notification.Id;
    }
}

public class MarkNotificationAsReadCommandHandler : ICommandHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly INotificationRepository _repository;

    public MarkNotificationAsReadCommandHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(MarkNotificationAsReadCommand command)
    {
        var notification = await _repository.GetByIdAsync(command.Id) ?? throw new Exception("Không tìm thấy Notification.");

        if (notification.UserId.Value.ToString() != command.UserId)
            throw new UnauthorizedAccessException("Bạn không có quyền sửa thông báo này.");

        notification.MarkAsRead();
        await _repository.UpdateAsync(notification);
        return true;
    }
}
