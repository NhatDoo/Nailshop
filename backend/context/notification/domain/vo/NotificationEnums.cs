namespace backend.context.notification.domain.vo;

public enum NotificationType
{
    Reminder,
    System,
    Todo
}

public enum NotificationChannel
{
    InApp,
    Email,
    Push
}

public enum NotificationEntityType
{
    Todo,
    Booking
}
