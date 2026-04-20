using backend.context.notification.application.commands;
using backend.context.notification.application.queries;
using backend.context.notification.application.services;
using backend.context.notification.domain.repo;
using backend.context.notification.infrastructure.persistence;
using backend.context.notification.infrastructure.services;
using Microsoft.Extensions.DependencyInjection;

namespace backend.context.notification;

public static class NotificationModule
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services)
    {
        // Repository
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Services
        services.AddScoped<IEmailService, MailKitEmailService>();

        // Handlers
        services.AddScoped<CreateNotificationCommandHandler>();
        services.AddScoped<MarkNotificationAsReadCommandHandler>();
        services.AddScoped<GetMyNotificationsQueryHandler>();

        return services;
    }
}
